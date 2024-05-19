using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Platform.Configuration;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Storage;

namespace abremir.AllMyBricks.Platform.Services
{
    public class SecureStorageService(ISecureStorage secureStorage) : ISecureStorageService
    {
        public async Task<string> GetBricksetApiKey()
        {
            return await secureStorage.GetAsync(Constants.BricksetApiKeySecureStorageKey).ConfigureAwait(false);
        }

        public async Task<bool> IsBricksetApiKeyAcquired()
        {
            return !string.IsNullOrWhiteSpace(await GetBricksetApiKey().ConfigureAwait(false));
        }

        public async Task SaveBricksetApiKey(string bricksetApiKey)
        {
            if (!await IsBricksetApiKeyAcquired().ConfigureAwait(false))
            {
                await secureStorage.SetAsync(Constants.BricksetApiKeySecureStorageKey, bricksetApiKey).ConfigureAwait(false);
            }
        }

        public async Task<Identification> GetDeviceIdentification()
        {
            return JsonSerializer.Deserialize<Identification>(await GetRawDeviceIdentification().ConfigureAwait(false));
        }

        public async Task<bool> IsDeviceIdentificationCreated()
        {
            return !string.IsNullOrWhiteSpace(await GetRawDeviceIdentification().ConfigureAwait(false));
        }

        public async Task SaveDeviceIdentification(Identification deviceIdentification)
        {
            if (!await IsDeviceIdentificationCreated().ConfigureAwait(false))
            {
                await secureStorage.SetAsync(Constants.DeviceIdentificationSecureStorageKey, JsonSerializer.Serialize(deviceIdentification)).ConfigureAwait(false);
            }
        }

        public async Task<string> GetBricksetUserHash(string username)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers().ConfigureAwait(false))?[username];
        }

        public async Task SaveBricksetPrimaryUser(string username, string userHash)
        {
            var bricksetUsers = JsonSerializer.Deserialize<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers().ConfigureAwait(false)) ?? [];

            if (bricksetUsers.TryAdd(username, userHash))
            {
                await secureStorage.SetAsync(Constants.BricksetPrimaryUsersStorageKey, JsonSerializer.Serialize(bricksetUsers)).ConfigureAwait(false);
            }
        }

        public async Task<bool> ClearBricksetPrimaryUser(string username)
        {
            var bricksetUsers = JsonSerializer.Deserialize<Dictionary<string, string>>(await GetRawBricksetPrimaryUsers().ConfigureAwait(false)) ?? [];

            if (!bricksetUsers.ContainsKey(username))
            {
                return false;
            }

            bricksetUsers.Remove(username);

            await secureStorage.SetAsync(Constants.BricksetPrimaryUsersStorageKey, JsonSerializer.Serialize(bricksetUsers)).ConfigureAwait(false);

            return true;
        }

        public async Task<bool> IsBricksetPrimaryUsersDefined()
        {
            return !string.IsNullOrWhiteSpace(await GetRawBricksetPrimaryUsers().ConfigureAwait(false));
        }

        public async Task<string> GetDefaultUsername()
        {
            return await secureStorage.GetAsync(Constants.DefaultUsernameStorageKey).ConfigureAwait(false);
        }

        public async Task<bool> IsDefaultUsernameDefined()
        {
            return !string.IsNullOrWhiteSpace(await GetDefaultUsername().ConfigureAwait(false));
        }

        public async Task SaveDefaultUsername(string username)
        {
            if (!await IsDefaultUsernameDefined().ConfigureAwait(false))
            {
                await secureStorage.SetAsync(Constants.DefaultUsernameStorageKey, username).ConfigureAwait(false);
            }
        }

        private async Task<string> GetRawDeviceIdentification()
        {
            return await secureStorage.GetAsync(Constants.DeviceIdentificationSecureStorageKey).ConfigureAwait(false);
        }

        private async Task<string> GetRawBricksetPrimaryUsers()
        {
            return await secureStorage.GetAsync(Constants.BricksetPrimaryUsersStorageKey).ConfigureAwait(false);
        }
    }
}
