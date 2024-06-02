using System;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using abremir.AllMyBricks.UserManagement.Interfaces;

namespace abremir.AllMyBricks.UserManagement.Services
{
    public class UserService(
        IBricksetApiService bricksetApiService,
        IBricksetUserRepository bricksetUserRepository,
        ISecureStorageService secureStorageService,
        IUserSynchronizationService userSynchronizationService) : IUserService
    {
        private readonly IBricksetApiService _bricksetApiService = bricksetApiService;
        private readonly IBricksetUserRepository _bricksetUserRepository = bricksetUserRepository;
        private readonly ISecureStorageService _secureStorageService = secureStorageService;
        private readonly IUserSynchronizationService _userSynchronizationService = userSynchronizationService;

        public async Task<bool> AddDefaultUser()
        {
            if (await _secureStorageService.IsDefaultUsernameDefined().ConfigureAwait(false))
            {
                return false;
            }

            var defaultUsername = Convert.ToBase64String(SHA256Hash.ComputeHash(Guid.NewGuid().ToString()));

            await _secureStorageService.SaveDefaultUsername(defaultUsername).ConfigureAwait(false);

            await _bricksetUserRepository.Add(BricksetUserType.None, defaultUsername).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> AddBricksetPrimaryUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || !await _secureStorageService.IsBricksetApiKeyAcquired().ConfigureAwait(false)
                || await _secureStorageService.IsBricksetPrimaryUsersDefined().ConfigureAwait(false)
                || await _bricksetUserRepository.Exists(username).ConfigureAwait(false))
            {
                return false;
            }

            var bricksetUserHash = await _bricksetApiService.Login(new ParameterLogin
            {
                Username = username,
                Password = password,
                ApiKey = await _secureStorageService.GetBricksetApiKey().ConfigureAwait(false)
            }).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(bricksetUserHash))
            {
                return false;
            }

            await _secureStorageService.SaveBricksetPrimaryUser(username, bricksetUserHash).ConfigureAwait(false);

            await _bricksetUserRepository.Add(BricksetUserType.Primary, username).ConfigureAwait(false);

            await _userSynchronizationService.SynchronizeBricksetPrimaryUsersSets(username).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> AddBricksetFriend(string username)
        {
            if (string.IsNullOrWhiteSpace(username)
                || !await _secureStorageService.IsBricksetApiKeyAcquired().ConfigureAwait(false)
                || await _bricksetUserRepository.Exists(username).ConfigureAwait(false))
            {
                return false;
            }

            await _bricksetUserRepository.Add(BricksetUserType.Friend, username).ConfigureAwait(false);

            await _userSynchronizationService.SynchronizeBricksetFriendsSets(username).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> RemoveBricksetPrimaryUser(string username)
        {
            Task<bool>[] tasks = [
                RemoveBricksetUser(username),
                _secureStorageService.ClearBricksetPrimaryUser(username)
            ];

            return (await Task.WhenAll(tasks).ConfigureAwait(false)).Aggregate(false, (seed, value) => seed || value);
        }

        public async Task<bool> RemoveBricksetFriend(string username)
        {
            return await RemoveBricksetUser(username).ConfigureAwait(false);
        }

        private async Task<bool> RemoveBricksetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username)
                || !await _bricksetUserRepository.Exists(username).ConfigureAwait(false))
            {
                return false;
            }

            await _bricksetUserRepository.Remove(username).ConfigureAwait(false);

            return true;
        }
    }
}
