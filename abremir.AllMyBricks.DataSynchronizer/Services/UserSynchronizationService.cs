using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class UserSynchronizationService : IUserSynchronizationService
    {
        private readonly IBricksetUserRepository _bricksetUserRepository;
        private readonly IOnboardingService _onboardingService;
        private readonly IUserSynchronizer _userSynchronizer;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IMessageHub _messageHub;

        public UserSynchronizationService(
            IBricksetUserRepository bricksetUserRepository,
            IOnboardingService onboardingService,
            IUserSynchronizer userSynchronizer,
            ISecureStorageService secureStorageService,
            IMessageHub messageHub)
        {
            _bricksetUserRepository = bricksetUserRepository;
            _onboardingService = onboardingService;
            _userSynchronizer = userSynchronizer;
            _secureStorageService = secureStorageService;
            _messageHub = messageHub;
        }

        public async Task SynchronizeBricksetPrimaryUsersSets(string username = null)
        {
            _messageHub.Publish(new UserSynchronizationServiceStart { UserType = BricksetUserTypeEnum.Primary });

            try
            {
                var apiKey = await _onboardingService.GetBricksetApiKey();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    var tasks = new List<Task>();

                    _bricksetUserRepository
                        .GetAllUsernames(BricksetUserTypeEnum.Primary)
                        .ToList()
                        .ForEach(bricksetUsername => tasks.Add(SynchronizeBricksetPrimaryUser(apiKey, bricksetUsername)));

                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserTypeEnum.Primary, Count = tasks.Count });

                    Task.WaitAll(tasks.ToArray());
                }
                else if (_bricksetUserRepository.Exists(username))
                {
                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserTypeEnum.Primary, Count = 1 });

                    await SynchronizeBricksetPrimaryUser(apiKey, username);
                }
                else
                {
                    throw new ArgumentException("Parameter was not found the list of primary users", nameof(username));
                }
            }
            catch (AggregateException aggEx)
            {
                _messageHub.Publish(new UserSynchronizationServiceException { UserType = BricksetUserTypeEnum.Primary, Exceptions = aggEx.InnerExceptions });
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new UserSynchronizationServiceException { UserType = BricksetUserTypeEnum.Primary, Exceptions = new[] { ex } });
            }

            _messageHub.Publish(new UserSynchronizationServiceEnd { UserType = BricksetUserTypeEnum.Primary });
        }

        public async Task SynchronizeBricksetFriendsSets(string username = null)
        {
            _messageHub.Publish(new UserSynchronizationServiceStart { UserType = BricksetUserTypeEnum.Friend });

            try
            {
                var apiKey = await _onboardingService.GetBricksetApiKey();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    var tasks = new List<Task>();

                    _bricksetUserRepository
                        .GetAllUsernames(BricksetUserTypeEnum.Friend)
                        .ToList()
                        .ForEach(bricksetUsername => tasks.Add(SynchronizeBricksetFriend(apiKey, bricksetUsername)));

                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserTypeEnum.Friend, Count = tasks.Count });

                    await Task.WhenAll(tasks);
                }
                else if (_bricksetUserRepository.Exists(username))
                {
                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserTypeEnum.Friend, Count = 1 });

                    await SynchronizeBricksetFriend(apiKey, username);
                }
                else
                {
                    throw new ArgumentException("Parameter was not found in the list of friends", nameof(username));
                }
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new UserSynchronizationServiceException { UserType = BricksetUserTypeEnum.Friend, Exceptions = new[] { ex } });
            }

            _messageHub.Publish(new UserSynchronizationServiceEnd { UserType = BricksetUserTypeEnum.Friend });
        }

        private async Task SynchronizeBricksetPrimaryUser(string apiKey, string username)
        {
            var userHash = await _secureStorageService.GetBricksetUserHash(username);

            if (string.IsNullOrWhiteSpace(userHash))
            {
                throw new Exception($"Invalid user hash for primary user '{username}'");
            }

            await _userSynchronizer.SynchronizeBricksetPrimaryUser(apiKey, username, userHash);
        }

        private async Task SynchronizeBricksetFriend(string apiKey, string username)
        {
            await _userSynchronizer.SynchronizeBricksetFriend(apiKey, username);
        }
    }
}
