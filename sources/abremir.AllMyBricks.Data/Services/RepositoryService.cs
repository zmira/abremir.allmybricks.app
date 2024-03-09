using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IMigrationRunner _migrationRunner;

        public RepositoryService(
            IFileSystemService fileSystemService,
            IMigrationRunner migrationRunner)
        {
            _fileSystemService = fileSystemService;
            _migrationRunner = migrationRunner;
        }

        public ILiteRepository GetRepository()
        {
            var repository = new LiteRepository(_fileSystemService.GetStreamForLocalPathToFile(Constants.AllMyBricksDbFile));

            _migrationRunner.ApplyMigrations(repository.Database);

            return repository;
        }

        public long CompactRepository()
        {
            using var repository = GetRepository();

            return repository.Database.Rebuild();
        }
    }
}
