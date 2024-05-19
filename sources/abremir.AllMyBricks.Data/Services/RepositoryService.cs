using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using LiteDB.async;
using LiteDB.Async;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService(
        IFileSystemService fileSystemService,
        IMigrationRunner migrationRunner)
        : IRepositoryService
    {
        public ILiteRepositoryAsync GetRepository()
        {
            var repository = new LiteRepositoryAsync(fileSystemService.GetStreamForLocalPathToFile(Constants.AllMyBricksDbFile));

            migrationRunner.ApplyMigrations(repository.Database);

            return repository;
        }

        public async Task<long> CompactRepository()
        {
            using var repository = GetRepository();

            return await repository.Database.RebuildAsync().ConfigureAwait(false);
        }
    }
}
