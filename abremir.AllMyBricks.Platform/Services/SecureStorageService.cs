using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Platform.Configuration;
using abremir.AllMyBricks.Platform.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Platform.Services
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
            return await _secureStorage.GetAsync(Constants.BricksetApiKeySecureStorageKey).ConfigureAwait(false);
        }

        public async Task<bool> IsBricksetApiKeyAcquired()
        {
            return !string.IsNullOrWhiteSpace(await GetBricksetApiKey().ConfigureAwait(false));
        }

        public async Task SaveBricksetApiKey(string bricksetApiKey)
        {
            if (!await IsBricksetApiKeyAcquired().ConfigureAwait(false))
            {
                await _secureStorage.SetAsync(Constants.BricksetApiKeySecureStorageKey, bricksetApiKey).ConfigureAwait(false);
            }
        }

        public async Task<Identification> GetDeviceIdentification()
        {
            return JsonConvert.DeserializeObject<Identification>(await GetRawDeviceIdentification().ConfigureAwait(false));
        }

        public async Task<bool> IsDeviceIdentificationCreated()
        {
            return !string.IsNullOrWhiteSpace(await GetRawDeviceIdentification().ConfigureAwait(false));
        }

        public async Task SaveDeviceIdentification(Identification deviceIdentification)
        {
            if (!await IsDeviceIdentificationCreated().ConfigureAwait(false))
            {
                await _secureStorage.SetAsync(Constants.DeviceIdentificationSecureStorageKey, JsonConvert.SerializeObject(deviceIdentification)).ConfigureAwait(false);
            }
        }

        public async Task<string> GetBricksetUserHash(string username)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers().ConfigureAwait(false))?[username];
        }

        public async Task SaveBricksetPrimaryUser(string username, string userHash)
        {
            var bricksetUsers = JsonConvert.DeserializeObject<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers().ConfigureAwait(false)) ?? new Dictionary<string, string>();

            if (!bricksetUsers.ContainsKey(username))
            {
                bricksetUsers.Add(username, userHash);

                await _secureStorage.SetAsync(Constants.BricksetPrimaryUsersStorageKey, JsonConvert.SerializeObject(bricksetUsers)).ConfigureAwait(false);
            }
        }

        public async Task<bool> ClearBricksetPrimaryUser(string username)
        {
            var bricksetUsers = JsonConvert.DeserializeObject<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers().ConfigureAwait(false)) ?? new Dictionary<string, string>();

            if (bricksetUsers.ContainsKey(username))
            {
                bricksetUsers.Remove(username);

                await _secureStorage.SetAsync(Constants.BricksetPrimaryUsersStorageKey, JsonConvert.SerializeObject(bricksetUsers)).ConfigureAwait(false);
            }

            return bricksetUsers.ContainsKey(username);
        }

        public async Task<bool> IsBricksetPrimaryUsersDefined()
        {
            return !string.IsNullOrWhiteSpace(await GetRawBricksetPrimaryUsers().ConfigureAwait(false));
        }

        public async Task<string> GetDefaultUsername()
        {
            return await _secureStorage.GetAsync(Constants.DefaultUsernameStorageKey).ConfigureAwait(false);
        }

        public async Task<bool> IsDefaultUsernameDefined()
        {
            return !string.IsNullOrWhiteSpace(await GetDefaultUsername().ConfigureAwait(false));
        }

        public async Task SaveDefaultUsername(string username)
        {
            if (!await IsDefaultUsernameDefined().ConfigureAwait(false))
            {
                await _secureStorage.SetAsync(Constants.DefaultUsernameStorageKey, username).ConfigureAwait(false);
            }
        }

        private async Task<string> GetRawDeviceIdentification()
        {
            return await _secureStorage.GetAsync(Constants.DeviceIdentificationSecureStorageKey).ConfigureAwait(false);
        }

        private async Task<string> GetRawBricksetPrimaryUsers()
        {
            return await _secureStorage.GetAsync(Constants.BricksetPrimaryUsersStorageKey).ConfigureAwait(false);
        }
    }
}
