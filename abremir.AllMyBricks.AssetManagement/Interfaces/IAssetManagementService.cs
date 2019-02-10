using System.Threading.Tasks;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IAssetManagementService
    {
        Task<bool> InstallAllMyBricksSeedDatabase(string databaseSeedUrl, string destinationFolderPath);
    }
}
