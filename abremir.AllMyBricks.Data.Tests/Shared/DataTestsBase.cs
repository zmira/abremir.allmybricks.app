using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Tests.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace abremir.AllMyBricks.Data.Tests.Shared
{
    public class DataTestsBase
    {
        protected static readonly IRepositoryService MemoryRepositoryService = new TestRepositoryService();

        private void ResetDatabase()
        {
            (MemoryRepositoryService as IMemoryRepositoryService)?.ResetDatabase();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            ResetDatabase();
        }

        protected static T InsertData<T>(T dataToInsert)
        {
            return InsertData(new[] { dataToInsert })[0];
        }

        protected static T[] InsertData<T>(T[] dataToInsert)
        {
            using var repository = MemoryRepositoryService.GetRepository();

            repository.Insert<T>(dataToInsert);

            return dataToInsert;
        }
    }
}
