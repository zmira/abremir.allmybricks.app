using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Interfaces;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        public async Task<string> GetBricksetApiKey()
        {
            return Settings.BricksetApiKey;
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
            return Settings.DeviceIdentification;
        }

        public async Task<bool> IsDeviceIdentificationCreated()
        {
            return await GetDeviceIdentification() != null;
        }

        public async Task SaveDeviceIdentification(Identification deviceIdentification)
        {
            await Task.Run(() => Settings.DeviceIdentification = deviceIdentification);
        }
    }
}