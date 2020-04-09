using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Migrations;
using abremir.AllMyBricks.Platform.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IFileSystemService _fileSystemService;

        public RepositoryService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public ILiteRepository GetRepository()
        {
            var repository = new LiteRepository(_fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile));

            RunMigrationsAndSetupIndexes(repository.Database);

            return repository;
        }

        public long CompactRepository()
        {
            using var repository = GetRepository();

            return repository.Database.Rebuild();
        }

        public static void RunMigrationsAndSetupIndexes(ILiteDatabase liteDatabase)
        {
            switch (liteDatabase.UserVersion)
            {
                case 0:
                    MigrationsAndIndexes_V0.Apply(liteDatabase);
                    break;
                case 1:
                    MigrationsAndIndexes_V1.Apply(liteDatabase);
                    break;
            }
        }
    }
}
