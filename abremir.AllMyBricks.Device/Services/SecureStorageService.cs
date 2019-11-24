using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using fastJSON;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        private readonly ISecureStorage _secureStorage;

        public SecureStorageService(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage;
        }

        public async Task<string> GetBricksetApiKey()
        {
            return await _secureStorage.GetAsync(Constants.BricksetApiKeySecureStorageKey);
        }

        public async Task<bool> IsBricksetApiKeyAcquired()
        {
            return !string.IsNullOrWhiteSpace(await GetBricksetApiKey());
        }

        public async Task SaveBricksetApiKey(string bricksetApiKey)
        {
            if (!await IsBricksetApiKeyAcquired())
            {
                await _secureStorage.SetAsync(Constants.BricksetApiKeySecureStorageKey, bricksetApiKey);
            }
        }

        public async Task<Identification> GetDeviceIdentification()
        {
            return JSON.ToObject<Identification>(await GetRawDeviceIdentification());
        }

        public async Task<bool> IsDeviceIdentificationCreated()
        {
            return !string.IsNullOrWhiteSpace(await GetRawDeviceIdentification());
        }

        public async Task SaveDeviceIdentification(Identification deviceIdentification)
        {
            if (!await IsDeviceIdentificationCreated())
            {
                await _secureStorage.SetAsync(Constants.DeviceIdentificationSecureStorageKey, JSON.ToJSON(deviceIdentification));
            }
        }

        public async Task<string> GetBricksetUserHash(string username)
        {
            return JSON.ToObject<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers())?[username];
        }

        public async Task SaveBricksetPrimaryUser(string username, string userHash)
        {
            var bricksetUsers = JSON.ToObject<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers()) ?? new Dictionary<string, string>();

            if (!bricksetUsers.ContainsKey(username))
            {
                bricksetUsers.Add(username, userHash);

                await _secureStorage.SetAsync(Constants.BricksetPrimaryUsersStorageKey, JSON.ToJSON(bricksetUsers));
            }
        }

        public async Task<bool> ClearBricksetPrimaryUser(string username)
        {
            var bricksetUsers = JSON.ToObject<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers()) ?? new Dictionary<string, string>();

            if (bricksetUsers.ContainsKey(username))
            {
                bricksetUsers.Remove(username);

                await _secureStorage.SetAsync(Constants.BricksetPrimaryUsersStorageKey, JSON.ToJSON(bricksetUsers));
            }

            return bricksetUsers.ContainsKey(username);
        }

        public async Task<bool> IsBricksetPrimaryUsersDefined()
        {
            return !string.IsNullOrWhiteSpace(await GetRawBricksetPrimaryUsers());
        }

        public async Task<string> GetDefaultUsername()
        {
            return await _secureStorage.GetAsync(Constants.DefaultUsernameStorageKey);
        }

        public async Task<bool> IsDefaultUsernameDefined()
        {
            return !string.IsNullOrWhiteSpace(await GetDefaultUsername());
        }

        public async Task SaveDefaultUsername(string username)
        {
            if (!await IsDefaultUsernameDefined())
            {
                await _secureStorage.SetAsync(Constants.DefaultUsernameStorageKey, username);
            }
        }

        private async Task<string> GetRawDeviceIdentification()
        {
            return await _secureStorage.GetAsync(Constants.DeviceIdentificationSecureStorageKey);
        }

        private async Task<string> GetRawBricksetPrimaryUsers()
        {
            return await _secureStorage.GetAsync(Constants.BricksetPrimaryUsersStorageKey);
        }
    }
}
