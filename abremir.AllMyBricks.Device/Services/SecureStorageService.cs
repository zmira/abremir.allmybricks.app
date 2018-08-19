using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
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

        public bool BricksetApiKeyAcquired => GetBricksetApiKey() != null;

        public bool DeviceIdentificationCreated => _secureStorage
            .GetAsync(Constants.DeviceIdentificationSecureStorageKey).Result != null;

        public string GetBricksetApiKey()
        {
            return _secureStorage
            .GetAsync(Constants.BricksetApiKeySecureStorageKey).Result;
        }

        public void SaveBricksetApiKey(string bricksetApiKey)
        {
            if (!BricksetApiKeyAcquired)
            {
                _secureStorage.SetAsync(Constants.BricksetApiKeySecureStorageKey, bricksetApiKey);
            }
        }

        public Identification GetDeviceIdentification()
        {
            return fastJSON.JSON.ToObject<Identification>(_secureStorage.GetAsync(Constants.DeviceIdentificationSecureStorageKey).Result);
        }

        public void SaveDeviceIdentification(Identification deviceIdentification)
        {
            if(!DeviceIdentificationCreated)
            {
                _secureStorage.SetAsync(Constants.DeviceIdentificationSecureStorageKey, fastJSON.JSON.ToJSON(deviceIdentification));
            }
        }
    }
}