using abremir.AllMyBricks.Core.Models;

namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface ISecureStorageService
    {
        bool BricksetApiKeyAcquired { get; }
        bool DeviceIdentificationCreated { get; }

        string GetBricksetApiKey();
        void SaveBricksetApiKey(string bricksetApiKey);
        Identification GetDeviceIdentification();
        void SaveDeviceIdentification(Identification deviceIdentification);
    }
}