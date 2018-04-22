using abremir.AllMyBricks.Data.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Tests.Configuration
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

        protected IList<T> GetAllData<T>()
        {
            using(var repository = MemoryRepositoryService.GetRepository())
            {
                return repository.Query<T>().ToList();
            }
        }
    }
}