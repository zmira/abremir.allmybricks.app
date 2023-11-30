using System.IO;
using System.Reflection;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Shared
{
    public class DataSynchronizerTestsBase
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        protected static readonly IRepositoryService MemoryRepositoryService = new TestRepositoryService();

        [TestInitialize]
        public void TestInitialize()
        {
            ResetDatabase();
        }

        protected string GetResultFileFromResource(string fileName)
        {
            var resourcePath = $"{GetAssemblyName()}.BricksetApiServiceResponses.{fileName}.json";

            using Stream stream = _assembly.GetManifestResourceStream(resourcePath);
            using var streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }

        private string GetAssemblyName()
        {
            return _assembly.GetName().Name;
        }

        private void ResetDatabase()
        {
            (MemoryRepositoryService as IMemoryRepositoryService)?.ResetDatabase();
        }
    }
}
