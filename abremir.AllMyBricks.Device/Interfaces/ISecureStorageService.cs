namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface ISecureStorageService
    {
        bool BricksetApiKeyAcquired { get; }
        string GetBricksetApiKey();
        void SaveBricksetApiKey(string bricksetApiKey);
    }
}