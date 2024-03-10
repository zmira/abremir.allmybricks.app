using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public interface IAssetManagementService
    {
        void CompressDatabaseFile(bool encrypted);
        void ExpandDatabaseFile(bool encrypted);
        Task CompactAllMyBricksDatabase();
        bool DatabaseFilePathExists();
        bool CompressedDatabaseFilePathExists(bool encrypted);
    }
}
