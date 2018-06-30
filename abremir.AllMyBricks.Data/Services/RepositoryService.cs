using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Providers;
using Realms;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IFilePathProvider _filePathProvider;

        public RepositoryService(IFilePathProvider filePathProvider)
        {
            _filePathProvider = filePathProvider;
        }

        public Realm GetRepository()
        {
            return Realm.GetInstance(_filePathProvider.GetLocalPathToFile(Constants.AllMyBricksDbFile));
        }
    }
}