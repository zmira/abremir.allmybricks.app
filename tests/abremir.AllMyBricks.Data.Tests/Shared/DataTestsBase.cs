using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Tests.Shared.Services;

namespace abremir.AllMyBricks.Data.Tests.Shared
{
    public class DataTestsBase : TestRepositoryBase
    {
        protected override IRepositoryService MemoryRepositoryService { get; set; } = new TestRepositoryService("abremir.AllMyBricks.Data.Tests.litedb");

        protected async Task<T> InsertData<T>(T dataToInsert)
        {
            return (await InsertData([dataToInsert]))[0];
        }

        protected async Task<T[]> InsertData<T>(T[] dataToInsert)
        {
            using var repository = MemoryRepositoryService.GetRepository();

            await repository.InsertAsync<T>(dataToInsert);

            return dataToInsert;
        }
    }
}
