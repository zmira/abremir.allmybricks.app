using abremir.AllMyBricks.Core.Models;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface ISecureStorageService
    {
        Task<string> GetBricksetApiKey();
        Task<bool> IsBricksetApiKeyAcquired();
        Task SaveBricksetApiKey(string bricksetApiKey);
        Task<Identification> GetDeviceIdentification();
        Task<bool> IsDeviceIdentificationCreated();
        Task SaveDeviceIdentification(Identification deviceIdentification);
    }
}