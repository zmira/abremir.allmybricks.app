using abremir.AllMyBricks.Data.Interfaces;
using Realms;

namespace abremir.AllMyBricks.Data.Tests.Configuration
{
    public class RepositoryService : IRepositoryService, IMemoryRepositoryService
    {
        private Realm _repository;

        public Realm GetRepository()
        {
            return _repository ?? (_repository = Realm.GetInstance(new InMemoryConfiguration("abremir.AllMyBricks.Data.Tests.realm")));
        }

        public void ResetDatabase()
        {
            _repository?.Dispose();
            _repository = null;
        }
    }
}