using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public interface IAssetManagementService
    {
        Task<int> CompressDatabaseFile(bool encrypted);
        Task<int> ExpandDatabaseFile(bool encrypted);
        Task<int> CompactAllMyBricksDatabase();
        bool DatabaseFilePathExists();
        bool CompressedDatabaseFilePathExists(bool encrypted);
    }
}
