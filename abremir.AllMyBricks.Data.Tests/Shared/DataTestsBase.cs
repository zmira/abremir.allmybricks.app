using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Tests.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Realms;
using System.Linq;

namespace abremir.AllMyBricks.Data.Tests.Shared
{
    public class DataTestsBase
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

        protected T InsertData<T>(T dataToInsert) where T : RealmObject
        {
            return InsertData(new[] { dataToInsert }).First();
        }

        protected T[] InsertData<T>(T[] dataToInsert) where T : RealmObject
        {
            var realm = MemoryRepositoryService.GetRepository();

            realm.Write(() =>
            {
                foreach (T data in dataToInsert)
                {
                    realm.Add(data);
                }
            });

            return dataToInsert;
        }
    }
}