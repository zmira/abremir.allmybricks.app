using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Shared.Models;

namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface ISecureStorageService
    {
        Task<string> GetBricksetApiKey();
        Task<bool> IsBricksetApiKeyAcquired();
        Task SaveBricksetApiKey(string bricksetApiKey);
        Task<Identification> GetDeviceIdentification();
        Task<bool> IsDeviceIdentificationCreated();
        Task SaveDeviceIdentification(Identification deviceIdentification);
        Task<string> GetBricksetUserHash(string username);
        Task<bool> IsBricksetPrimaryUsersDefined();
        Task SaveBricksetPrimaryUser(string username, string userHash);
        Task<bool> ClearBricksetPrimaryUser(string username);
        Task<string> GetDefaultUsername();
        Task<bool> IsDefaultUsernameDefined();
        Task SaveDefaultUsername(string username);
    }
}
