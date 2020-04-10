using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Services;
using LiteDB;
using LiteDB.Engine;

namespace abremir.AllMyBricks.Data.Tests.Configuration
{
    public class TestRepositoryService : IRepositoryService, IMemoryRepositoryService
    {
        private TempStream _tempStream;

        public long CompactRepository()
        {
            throw new System.NotImplementedException();
        }

        public ILiteRepository GetRepository()
        {
            if (_tempStream is null)
            {
                _tempStream = new TempStream("abremir.AllMyBricks.Data.Tests.litedb");
            }

            var liteRepository = new LiteRepository(_tempStream);

            RepositoryService.RunMigrationsAndSetupIndexes(liteRepository.Database);

            return liteRepository;
        }

        public void ResetDatabase()
        {
            _tempStream?.Dispose();
            _tempStream = null;
        }
    }
}
