using System.Threading.Tasks;
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

        protected static async Task<T> InsertData<T>(T dataToInsert)
        {
            return (await InsertData(new[] { dataToInsert }))[0];
        }

        protected static async Task<T[]> InsertData<T>(T[] dataToInsert)
        {
            using var repository = MemoryRepositoryService.GetRepository();

            await repository.InsertAsync<T>(dataToInsert);

            return dataToInsert;
        }
    }
}
