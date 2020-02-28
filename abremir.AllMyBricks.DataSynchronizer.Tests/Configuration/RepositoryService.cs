using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;
using LiteDB.Engine;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Configuration
{
    public class RepositoryService : IRepositoryService, IMemoryRepositoryService
    {
        private TempStream _tempStream;

        public long CompactRepository()
        {
            throw new System.NotImplementedException();
        }

        public ILiteRepository GetRepository()
        {
            if (_tempStream == null)
            {
                _tempStream = new TempStream("abremir.AllMyBricks.DataSynchronizer.Tests.litedb");
            }
            return new LiteRepository(_tempStream);
        }

        public void ResetDatabase()
        {
            _tempStream?.Dispose();
            _tempStream = null;
        }
    }
}
