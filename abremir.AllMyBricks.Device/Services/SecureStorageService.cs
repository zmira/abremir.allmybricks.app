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

        public bool BricksetApiKeyAcquired => _secureStorage
            .GetAsync(Constants.BricksetApiKeySecureStorageKey).Result != null;

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
    }
}