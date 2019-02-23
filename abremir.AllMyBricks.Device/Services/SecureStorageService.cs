using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using fastJSON;
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
            return await _secureStorage
            .GetAsync(Constants.BricksetApiKeySecureStorageKey);
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
            if(!await IsDeviceIdentificationCreated())
            {
                await _secureStorage.SetAsync(Constants.DeviceIdentificationSecureStorageKey, JSON.ToJSON(deviceIdentification));
            }
        }

        private async Task<string> GetRawDeviceIdentification()
        {
            return await _secureStorage.GetAsync(Constants.DeviceIdentificationSecureStorageKey);
        }
    }
}