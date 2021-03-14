using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using abremir.AllMyBricks.UserManagement.Interfaces;
using System;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly IBricksetUserRepository _bricksetUserRepository;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IUserSynchronizationService _userSynchronizationService;

        public UserService(
            IBricksetApiService bricksetApiService,
            IBricksetUserRepository bricksetUserRepository,
            ISecureStorageService secureStorageService,
            IUserSynchronizationService userSynchronizationService)
        {
            _bricksetApiService = bricksetApiService;
            _bricksetUserRepository = bricksetUserRepository;
            _secureStorageService = secureStorageService;
            _userSynchronizationService = userSynchronizationService;
        }

        public async Task<bool> AddDefaultUser()
        {
            if (await _secureStorageService.IsDefaultUsernameDefined().ConfigureAwait(false))
            {
                return false;
            }

            var defaultUsername = Convert.ToBase64String(SHA256Hash.ComputeHash(Guid.NewGuid().ToString()));

            await _secureStorageService.SaveDefaultUsername(defaultUsername).ConfigureAwait(false);

            _bricksetUserRepository.Add(BricksetUserType.None, defaultUsername);

            return true;
        }

        public async Task<bool> AddBricksetPrimaryUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || !await _secureStorageService.IsBricksetApiKeyAcquired().ConfigureAwait(false)
                || await _secureStorageService.IsBricksetPrimaryUsersDefined().ConfigureAwait(false)
                || _bricksetUserRepository.Exists(username))
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

            _bricksetUserRepository.Add(BricksetUserType.Primary, username);

            await _userSynchronizationService.SynchronizeBricksetPrimaryUsersSets(username).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> AddBricksetFriend(string username)
        {
            if (string.IsNullOrWhiteSpace(username)
                || !await _secureStorageService.IsBricksetApiKeyAcquired().ConfigureAwait(false)
                || _bricksetUserRepository.Exists(username))
            {
                return false;
            }

            _bricksetUserRepository.Add(BricksetUserType.Friend, username);

            await _userSynchronizationService.SynchronizeBricksetFriendsSets(username).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> RemoveBricksetPrimaryUser(string username)
        {
            if (RemoveBricksetUser(username))
            {
                return await _secureStorageService.ClearBricksetPrimaryUser(username).ConfigureAwait(false);
            }

            return false;
        }

        public bool RemoveBricksetFriend(string username)
        {
            return RemoveBricksetUser(username);
        }

        private bool RemoveBricksetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username)
                || !_bricksetUserRepository.Exists(username))
            {
                return false;
            }

            _bricksetUserRepository.Remove(username);

            return true;
        }
    }
}
