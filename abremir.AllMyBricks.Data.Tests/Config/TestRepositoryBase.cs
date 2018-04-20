using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Tests.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace abremir.AllMyBricks.Data.Tests.Config
{
    public class TestRepositoryBase
    {
        protected static readonly IRepositoryService MemoryRepositoryService = new RepositoryService();

        private void ResetDatabase()
        {
            (MemoryRepositoryService as IMemoryRepositoryService)?.ResetDatabase();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ResetDatabase();
        }

        protected void InsertData<T>(T dataToInsert)
        {
            InsertData(new[] { dataToInsert });
        }

        protected void InsertData<T>(T[] dataToInsert)
        {
            using (var repository = MemoryRepositoryService.GetRepository())
            {
                foreach (T data in dataToInsert)
                {
                    repository.Insert(data);
                }
            }
        }
    }
}