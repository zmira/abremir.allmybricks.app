using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Realms;
using System.Linq;

namespace abremir.AllMyBricks.Data.Tests.Configuration
{
    public class TestRepositoryBase
    {
        protected static readonly IRepositoryService MemoryRepositoryService = new RepositoryService();

        public TestRepositoryBase()
        {
            Mappings.Configure();
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