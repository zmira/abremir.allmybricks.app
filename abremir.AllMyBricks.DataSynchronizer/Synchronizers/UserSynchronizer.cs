using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using Easy.MessageHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class UserSynchronizer : IUserSynchronizer
    {
        private readonly IMessageHub _messageHub;
        private readonly IBricksetUserRepository _bricksetUserRepository;
        private readonly IBricksetApiService _bricksetApiService;

        public UserSynchronizer(
            IBricksetApiService bricksetApiService,
            IBricksetUserRepository bricksetUserRepository,
            IMessageHub messageHub)
        {
            _bricksetApiService = bricksetApiService;
            _bricksetUserRepository = bricksetUserRepository;
            _messageHub = messageHub;
        }

        public async Task SynchronizeBricksetPrimaryUser(string apiKey, string username, string userHash)
        {
            _messageHub.Publish(new UserSynchronizerStart { UserType = BricksetUserTypeEnum.Primary, Username = username });

            try
            {
                var user = _bricksetUserRepository.Get(username);

                if (user.UserSynchronizationTimestamp.HasValue)
                {
                    _messageHub.Publish(new AllMyBricksToBricksetStart());

                    _messageHub.Publish(new AllMyBricksToBricksetAcquiringSetsStart());

                    var updatedSetsSinceLastSynchronization = user.Sets.Where(set => set.LastChangeTimestamp > user.UserSynchronizationTimestamp.Value).ToList();

                    _messageHub.Publish(new AllMyBricksToBricksetAcquiringSetsEnd { Count = updatedSetsSinceLastSynchronization.Count });

                    updatedSetsSinceLastSynchronization.ForEach(set =>
                    {
                        _messageHub.Publish(new UserSynchronizerSynchronizingSetStart { SetId = set.SetId });

                        var setCollectionParameter = new ParameterSetCollection
                        {
                            ApiKey = apiKey,
                            UserHash = userHash,
                            SetID = set.SetId,
                            QtyOwned = set.QuantityOwned,
                            Wanted = set.Wanted ? 1 : 0
                        };

                        _bricksetApiService.SetCollection(setCollectionParameter);

                        _messageHub.Publish(new UserSynchronizerSynchronizingSetEnd { SetId = set.SetId });
                    });

                    _messageHub.Publish(new AllMyBricksToBricksetEnd());
                }

                _messageHub.Publish(new BricksetToAllMyBricksStart());

                _messageHub.Publish(new BricksetToAllMyBricksAcquiringSetsStart());

                var bricksetUserSets = await GetAllUserSetsFromBrickset(apiKey, userHash: userHash);

                var allMyBricksUserSetIds = user.Sets.Select(set => set.SetId);
                var bricksetUserSetIds = bricksetUserSets.Select(bricksetUserSet => bricksetUserSet.SetId);
                var setIdsNotInAllMyBricks = bricksetUserSetIds.Except(allMyBricksUserSetIds).ToList();

                _messageHub.Publish(new BricksetToAllMyBricksAcquiringSetsEnd { Count = setIdsNotInAllMyBricks.Count });

                foreach (var userSetNotInAllMyBricks in bricksetUserSets.Where(bricksetUserSet => setIdsNotInAllMyBricks.Contains(bricksetUserSet.SetId)))
                {
                    _messageHub.Publish(new UserSynchronizerSynchronizingSetStart { SetId = userSetNotInAllMyBricks.SetId });

                    _bricksetUserRepository.AddOrUpdateSet(username, userSetNotInAllMyBricks);

                    _messageHub.Publish(new UserSynchronizerSynchronizingSetEnd { SetId = userSetNotInAllMyBricks.SetId });
                }

                _messageHub.Publish(new BricksetToAllMyBricksEnd());

                _bricksetUserRepository.UpdateUserSynchronizationTimestamp(username, DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new UserSynchronizerException { UserType = BricksetUserTypeEnum.Primary, Username = username, Exception = ex });
            }

            _messageHub.Publish(new UserSynchronizerEnd { UserType = BricksetUserTypeEnum.Primary, Username = username });
        }

        public async Task SynchronizeBricksetFriend(string apiKey, string username)
        {
            _messageHub.Publish(new UserSynchronizerStart { UserType = BricksetUserTypeEnum.Friend, Username = username });

            try
            {
                _messageHub.Publish(new BricksetToAllMyBricksAcquiringSetsStart());

                var bricksetUserSets = (await GetAllUserSetsFromBrickset(apiKey, username)).ToList();

                _messageHub.Publish(new BricksetToAllMyBricksAcquiringSetsEnd { Count = bricksetUserSets.Count });

                foreach (var bricksetUserSet in bricksetUserSets)
                {
                    _messageHub.Publish(new UserSynchronizerSynchronizingSetStart { SetId = bricksetUserSet.SetId });

                    _bricksetUserRepository.AddOrUpdateSet(username, bricksetUserSet);

                    _messageHub.Publish(new UserSynchronizerSynchronizingSetEnd { SetId = bricksetUserSet.SetId });
                }

                _bricksetUserRepository.UpdateUserSynchronizationTimestamp(username, DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new UserSynchronizerException { UserType = BricksetUserTypeEnum.Friend, Username = username, Exception = ex });
            }

            _messageHub.Publish(new UserSynchronizerEnd { UserType = BricksetUserTypeEnum.Friend, Username = username });
        }

        private async Task<IEnumerable<BricksetUserSet>> GetAllUserSetsFromBrickset(string apiKey, string username = null, string userHash = null)
        {
            var allUserSets = await GetAllOwnedSets(apiKey, username, userHash);
            var allWantedSetIds = await GetAllWantedSetIds(apiKey, username, userHash);

            foreach (var wantedSetId in allWantedSetIds)
            {
                if (allUserSets.ContainsKey(wantedSetId))
                {
                    allUserSets[wantedSetId].Wanted = true;
                }
                else
                {
                    allUserSets.Add(wantedSetId, new BricksetUserSet
                    {
                        SetId = wantedSetId,
                        Wanted = true
                    });
                }
            }

            return allUserSets.Values;
        }

        private async Task<IDictionary<long, BricksetUserSet>> GetAllOwnedSets(string apiKey, string username = null, string userHash = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey)
                || (string.IsNullOrWhiteSpace(userHash) && string.IsNullOrWhiteSpace(username))
                || (!string.IsNullOrWhiteSpace(userHash) && !string.IsNullOrWhiteSpace(username)))
            {
                return new Dictionary<long, BricksetUserSet>();
            }

            var getSetsParameter = new ParameterSets
            {
                ApiKey = apiKey,
                UserHash = userHash ?? string.Empty,
                UserName = !string.IsNullOrWhiteSpace(userHash) ? string.Empty : username,
                Owned = "1",
                PageSize = Constants.BricksetPageSizeParameter.ToString()
            };

            var ownedSets = new List<BricksetUserSet>();
            var pageNumber = 1;
            var currentPageResults = new List<Sets>();
            do
            {
                getSetsParameter.PageNumber = pageNumber.ToString();

                currentPageResults = (await _bricksetApiService.GetSets(getSetsParameter)).ToList();

                ownedSets.AddRange(currentPageResults.Select(set => new BricksetUserSet
                {
                    SetId = set.SetId,
                    Owned = true,
                    QuantityOwned = (short)set.QtyOwned
                }));

                pageNumber++;
            } while (currentPageResults.Count == Constants.BricksetPageSizeParameter);

            return ownedSets.ToDictionary(bricksetUserSet => bricksetUserSet.SetId);
        }

        private async Task<IEnumerable<int>> GetAllWantedSetIds(string apiKey, string username = null, string userHash = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey)
                || (string.IsNullOrWhiteSpace(userHash) && string.IsNullOrWhiteSpace(username))
                || (!string.IsNullOrWhiteSpace(userHash) && !string.IsNullOrWhiteSpace(username)))
            {
                return new List<int>();
            }

            var getSetsParameter = new ParameterSets
            {
                ApiKey = apiKey,
                UserHash = userHash ?? string.Empty,
                UserName = !string.IsNullOrWhiteSpace(userHash) ? string.Empty : username,
                Wanted = "1",
                PageSize = Constants.BricksetPageSizeParameter.ToString()
            };

            var wantedSets = new List<int>();
            var pageNumber = 1;
            var currentPageResults = new List<Sets>();
            do
            {
                getSetsParameter.PageNumber = pageNumber.ToString();

                currentPageResults = (await _bricksetApiService.GetSets(getSetsParameter)).ToList();

                wantedSets.AddRange(currentPageResults.Select(set => set.SetId));

                pageNumber++;
            } while (currentPageResults.Count == Constants.BricksetPageSizeParameter);

            return wantedSets;
        }
    }
}
