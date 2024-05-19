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
        IUserSynchronizationService userSynchronizationService)
        : IUserService
    {
        public async Task<bool> AddDefaultUser()
        {
            if (await secureStorageService.IsDefaultUsernameDefined().ConfigureAwait(false))
            {
                return false;
            }

            var defaultUsername = Convert.ToBase64String(SHA256Hash.ComputeHash(Guid.NewGuid().ToString()));

            await secureStorageService.SaveDefaultUsername(defaultUsername).ConfigureAwait(false);

            await bricksetUserRepository.Add(BricksetUserType.None, defaultUsername).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> AddBricksetPrimaryUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || !await secureStorageService.IsBricksetApiKeyAcquired().ConfigureAwait(false)
                || await secureStorageService.IsBricksetPrimaryUsersDefined().ConfigureAwait(false)
                || await bricksetUserRepository.Exists(username).ConfigureAwait(false))
            {
                return false;
            }

            var bricksetUserHash = await bricksetApiService.Login(new ParameterLogin
            {
                Username = username,
                Password = password,
                ApiKey = await secureStorageService.GetBricksetApiKey().ConfigureAwait(false)
            }).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(bricksetUserHash))
            {
                return false;
            }

            await secureStorageService.SaveBricksetPrimaryUser(username, bricksetUserHash).ConfigureAwait(false);

            await bricksetUserRepository.Add(BricksetUserType.Primary, username).ConfigureAwait(false);

            await userSynchronizationService.SynchronizeBricksetPrimaryUsersSets(username).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> AddBricksetFriend(string username)
        {
            if (string.IsNullOrWhiteSpace(username)
                || !await secureStorageService.IsBricksetApiKeyAcquired().ConfigureAwait(false)
                || await bricksetUserRepository.Exists(username).ConfigureAwait(false))
            {
                return false;
            }

            await bricksetUserRepository.Add(BricksetUserType.Friend, username).ConfigureAwait(false);

            await userSynchronizationService.SynchronizeBricksetFriendsSets(username).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> RemoveBricksetPrimaryUser(string username)
        {
            Task<bool>[] tasks = [
                RemoveBricksetUser(username),
                secureStorageService.ClearBricksetPrimaryUser(username)
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
                || !await bricksetUserRepository.Exists(username).ConfigureAwait(false))
            {
                return false;
            }

            await bricksetUserRepository.Remove(username).ConfigureAwait(false);

            return true;
        }
    }
}
