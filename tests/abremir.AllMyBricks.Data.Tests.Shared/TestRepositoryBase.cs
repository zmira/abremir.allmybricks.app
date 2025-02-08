using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Tests.Shared.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace abremir.AllMyBricks.Data.Tests.Shared
{
    public abstract class TestRepositoryBase
    {
        protected abstract IRepositoryService MemoryRepositoryService
        {
            get;
            set;
        }

        private void ResetDatabase()
        {
            (MemoryRepositoryService as IMemoryRepositoryService)?.ResetDatabase();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ResetDatabase();
        }
    }
}
