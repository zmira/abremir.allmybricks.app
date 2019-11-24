using abremir.AllMyBricks.Data.Interfaces;
using Realms;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Configuration
{
    public class RepositoryService : IRepositoryService, IMemoryRepositoryService
    {
        private Realm _repository;

        public bool CompactRepository()
        {
            throw new System.NotImplementedException();
        }

        public Realm GetRepository()
        {
            return _repository ?? (_repository = Realm.GetInstance(new InMemoryConfiguration("abremir.AllMyBricks.DataSynchronizer.Tests.realm")));
        }

        public void ResetDatabase()
        {
            _repository?.Dispose();
            _repository = null;
        }
    }
}
