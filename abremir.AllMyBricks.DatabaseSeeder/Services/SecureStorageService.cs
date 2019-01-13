using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Interfaces;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        public bool BricksetApiKeyAcquired => GetBricksetApiKey() != null;

        public bool DeviceIdentificationCreated => GetDeviceIdentification() != null;

        public string GetBricksetApiKey()
        {
            return Settings.BricksetApiKey;
        }

        public Identification GetDeviceIdentification()
        {
            return Settings.DeviceIdentification;
        }

        public void SaveBricksetApiKey(string bricksetApiKey)
        {
            Settings.BricksetApiKey = bricksetApiKey;
        }

        public void SaveDeviceIdentification(Identification deviceIdentification)
        {
            Settings.DeviceIdentification = deviceIdentification;
        }
    }
}