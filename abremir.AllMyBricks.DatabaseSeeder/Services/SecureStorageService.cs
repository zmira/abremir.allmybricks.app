using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        public async Task<string> GetBricksetApiKey()
        {
            return await Task.Run(() => Settings.BricksetApiKey);
        }

        public async Task<bool> IsBricksetApiKeyAcquired()
        {
            return !string.IsNullOrWhiteSpace(await GetBricksetApiKey());
        }

        public async Task SaveBricksetApiKey(string bricksetApiKey)
        {
            await Task.Run(() => Settings.BricksetApiKey = bricksetApiKey);
        }

        public async Task<Identification> GetDeviceIdentification()
        {
            return await Task.Run(() => Settings.DeviceIdentification);
        }

        public async Task<bool> IsDeviceIdentificationCreated()
        {
            return await GetDeviceIdentification() != null;
        }

        public async Task SaveDeviceIdentification(Identification deviceIdentification)
        {
            await Task.Run(() => Settings.DeviceIdentification = deviceIdentification);
        }

        public async Task<string> GetBricksetUserHash(string username)
        {
            return await Task.Run(() => Settings.BricksetPrimaryUsers[username]);
        }

        public async Task<bool> IsBricksetPrimaryUsersDefined()
        {
            return await Task.Run(() => Settings.BricksetPrimaryUsers != null);
        }

        public async Task SaveBricksetPrimaryUser(string username, string userHash)
        {
            var bricksetPrimaryUsers = Settings.BricksetPrimaryUsers ?? new Dictionary<string, string>();

            if (!bricksetPrimaryUsers.ContainsKey(username))
            {
                bricksetPrimaryUsers.Add(username, userHash);

                await Task.Run(() => Settings.BricksetPrimaryUsers = bricksetPrimaryUsers);
            }
        }

        public async Task<bool> ClearBricksetPrimaryUser(string username)
        {
            var bricksetPrimaryUsers = Settings.BricksetPrimaryUsers ?? new Dictionary<string, string>();

            if (bricksetPrimaryUsers.ContainsKey(username))
            {
                bricksetPrimaryUsers.Remove(username);

                await Task.Run(() => Settings.BricksetPrimaryUsers = bricksetPrimaryUsers);
            }

            return bricksetPrimaryUsers.ContainsKey(username);
        }

        public Task<string> GetDefaultUsername()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsDefaultUsernameDefined()
        {
            throw new System.NotImplementedException();
        }

        public Task SaveDefaultUsername(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
