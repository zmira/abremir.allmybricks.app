using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Realms;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IFileSystemService _fileSystemService;

        public RepositoryService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public Realm GetRepository()
        {
            return Realm.GetInstance(_fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile));
        }

        public bool CompactRepository()
        {
            return Realm.Compact(new RealmConfiguration(_fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile)));
        }
    }
}
