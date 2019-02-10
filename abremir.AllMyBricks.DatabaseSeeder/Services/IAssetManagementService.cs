namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public interface IAssetManagementService
    {
        void CompressDatabaseFile();
        void UncompressDatabaseFile();
        void CompactAllMyBricksDatabase();
        bool DatabaseFilePathExists();
        bool CompressedDatabaseFilePathExists();
    }
}
