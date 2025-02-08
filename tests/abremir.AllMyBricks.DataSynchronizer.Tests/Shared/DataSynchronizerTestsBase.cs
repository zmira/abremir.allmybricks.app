using System.IO;
using System.Reflection;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Tests.Shared;
using abremir.AllMyBricks.Data.Tests.Shared.Services;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Shared
{
    public class DataSynchronizerTestsBase : TestRepositoryBase
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        protected override IRepositoryService MemoryRepositoryService { get; set; } = new TestRepositoryService("abremir.AllMyBricks.DataSynchronizer.Tests.litedb");

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
    }
}
