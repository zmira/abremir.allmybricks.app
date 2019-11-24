using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
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
            if (await _secureStorageService.IsDefaultUsernameDefined())
            {
                return false;
            }

            var defaultUsername = Convert.ToBase64String(SHA256Hash.ComputeHash(Guid.NewGuid().ToString()));

            await _secureStorageService.SaveDefaultUsername(defaultUsername);

            _bricksetUserRepository.Add(BricksetUserTypeEnum.None, defaultUsername);

            return true;
        }

        public async Task<bool> AddBricksetPrimaryUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || !await _secureStorageService.IsBricksetApiKeyAcquired()
                || await _secureStorageService.IsBricksetPrimaryUsersDefined()
                || _bricksetUserRepository.Exists(username))
            {
                return false;
            }

            var bricksetUserHash = await _bricksetApiService.Login(new ParameterLogin
            {
                Username = username,
                Password = password,
                ApiKey = await _secureStorageService.GetBricksetApiKey()
            });

            if (string.IsNullOrWhiteSpace(bricksetUserHash))
            {
                return false;
            }

            await _secureStorageService.SaveBricksetPrimaryUser(username, bricksetUserHash);

            _bricksetUserRepository.Add(BricksetUserTypeEnum.Primary, username);

            await _userSynchronizationService.SynchronizeBricksetPrimaryUsersSets(username);

            return true;
        }

        public async Task<bool> AddBricksetFriend(string username)
        {
            if (string.IsNullOrWhiteSpace(username)
                || !await _secureStorageService.IsBricksetApiKeyAcquired()
                || _bricksetUserRepository.Exists(username))
            {
                return false;
            }

            _bricksetUserRepository.Add(BricksetUserTypeEnum.Friend, username);

            await _userSynchronizationService.SynchronizeBricksetFriendsSets(username);

            return true;
        }

        public async Task<bool> RemoveBricksetPrimaryUser(string username)
        {
            if (RemoveBricksetUser(username))
            {
                return await _secureStorageService.ClearBricksetPrimaryUser(username);
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
