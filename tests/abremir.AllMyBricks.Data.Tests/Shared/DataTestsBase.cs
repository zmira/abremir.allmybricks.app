using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Tests.Shared.Interfaces;
using abremir.AllMyBricks.Data.Tests.Shared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace abremir.AllMyBricks.Data.Tests.Shared
{
    public class DataTestsBase
    {
        protected static readonly IRepositoryService MemoryRepositoryService = new TestRepositoryService("abremir.AllMyBricks.Data.Tests.litedb");

        private static void ResetDatabase()
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
            return (await InsertData([dataToInsert]))[0];
        }

        protected static async Task<T[]> InsertData<T>(T[] dataToInsert)
        {
            using var repository = MemoryRepositoryService.GetRepository();

            await repository.InsertAsync<T>(dataToInsert);

            return dataToInsert;
        }
    }
}
