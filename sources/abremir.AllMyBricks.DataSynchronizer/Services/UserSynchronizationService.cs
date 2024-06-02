using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class UserSynchronizationService(
        IBricksetUserRepository bricksetUserRepository,
        IOnboardingService onboardingService,
        IUserSynchronizer userSynchronizer,
        ISecureStorageService secureStorageService,
        IMessageHub messageHub)
        : IUserSynchronizationService
    {
        private readonly IBricksetUserRepository _bricksetUserRepository = bricksetUserRepository;
        private readonly IOnboardingService _onboardingService = onboardingService;
        private readonly IUserSynchronizer _userSynchronizer = userSynchronizer;
        private readonly ISecureStorageService _secureStorageService = secureStorageService;
        private readonly IMessageHub _messageHub = messageHub;

        public async Task SynchronizeBricksetPrimaryUsersSets(string username = null)
        {
            _messageHub.Publish(new UserSynchronizationServiceStart { UserType = BricksetUserType.Primary });

            try
            {
                var apiKey = await _onboardingService.GetBricksetApiKey().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    List<Task> tasks = [];

                    (await _bricksetUserRepository
                        .GetAllUsernames(BricksetUserType.Primary).ConfigureAwait(false))
                        .ToList()
                        .ForEach(bricksetUsername => tasks.Add(SynchronizeBricksetPrimaryUser(apiKey, bricksetUsername)));

                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserType.Primary, Count = tasks.Count });

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else if (await _bricksetUserRepository.Exists(username).ConfigureAwait(false))
                {
                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserType.Primary, Count = 1 });

                    await SynchronizeBricksetPrimaryUser(apiKey, username).ConfigureAwait(false);
                }
                else
                {
                    throw new ArgumentException("Parameter was not found the list of primary users", nameof(username));
                }
            }
            catch (AggregateException aggEx)
            {
                _messageHub.Publish(new UserSynchronizationServiceException { UserType = BricksetUserType.Primary, Exceptions = aggEx.InnerExceptions });
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new UserSynchronizationServiceException { UserType = BricksetUserType.Primary, Exceptions = [ex] });
            }

            _messageHub.Publish(new UserSynchronizationServiceEnd { UserType = BricksetUserType.Primary });
        }

        public async Task SynchronizeBricksetFriendsSets(string username = null)
        {
            _messageHub.Publish(new UserSynchronizationServiceStart { UserType = BricksetUserType.Friend });

            try
            {
                var apiKey = await _onboardingService.GetBricksetApiKey().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    List<Task> tasks = [];

                    (await _bricksetUserRepository
                        .GetAllUsernames(BricksetUserType.Friend).ConfigureAwait(false))
                        .ToList()
                        .ForEach(bricksetUsername => tasks.Add(SynchronizeBricksetFriend(apiKey, bricksetUsername)));

                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserType.Friend, Count = tasks.Count });

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else if (await _bricksetUserRepository.Exists(username).ConfigureAwait(false))
                {
                    _messageHub.Publish(new UsersAcquired { UserType = BricksetUserType.Friend, Count = 1 });

                    await SynchronizeBricksetFriend(apiKey, username).ConfigureAwait(false);
                }
                else
                {
                    throw new ArgumentException("Parameter was not found in the list of friends", nameof(username));
                }
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new UserSynchronizationServiceException { UserType = BricksetUserType.Friend, Exceptions = [ex] });
            }

            _messageHub.Publish(new UserSynchronizationServiceEnd { UserType = BricksetUserType.Friend });
        }

        private async Task SynchronizeBricksetPrimaryUser(string apiKey, string username)
        {
            var userHash = await _secureStorageService.GetBricksetUserHash(username).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(userHash))
            {
                throw new Exception($"Invalid user hash for primary user '{username}'");
            }

            await _userSynchronizer.SynchronizeBricksetPrimaryUser(apiKey, username, userHash).ConfigureAwait(false);
        }

        private async Task SynchronizeBricksetFriend(string apiKey, string username)
        {
            await _userSynchronizer.SynchronizeBricksetFriend(apiKey, username).ConfigureAwait(false);
        }
    }
}
