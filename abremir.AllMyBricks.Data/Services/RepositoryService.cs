using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
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

        public LiteRepository GetRepository()
        {
            var repository = new LiteRepository(_fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile));

            return repository;
        }

        public long CompactRepository()
        {
            return GetRepository().Database.Rebuild();
        }

        {
        }
    }
}
