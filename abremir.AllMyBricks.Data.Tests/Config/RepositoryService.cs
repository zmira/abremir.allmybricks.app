using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;
using System.IO;

namespace abremir.AllMyBricks.Data.Tests.Config
{
    public class RepositoryService : IRepositoryService, IMemoryRepositoryService
    {
        private LiteRepository _liteRepository;
        private LiteDatabase _liteDatabase;

        public LiteRepository GetRepository()
        {
            if (_liteDatabase == null)
            {
                _liteDatabase = new LiteDatabase(new MemoryStream());
            }

            if (_liteRepository == null || _liteRepository.Database == null)
            {
                _liteRepository = new LiteRepository(_liteDatabase);
            }

            Services.RepositoryService.SetupIndexes(_liteRepository.Engine);

            return _liteRepository;
        }

        public void ResetDatabase()
        {
            _liteDatabase = null;
        }
    }
}