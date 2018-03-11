using abremir.AllMyBricks.ThirdParty.Brickset.Extensions;
using Flurl.Http.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared
{
    public class BricksetApiServiceTestsBase
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        protected HttpTest _httpTestFake;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpTestFake = new HttpTest();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _httpTestFake.Dispose();
        }

        protected string GetResultFileFromResource(string fileName)
        {
            var resourcePath = $"{GetAssemblyName()}.ApiResponses.{GetType().GetDescription()}.{fileName}.xml";

            using (Stream stream = _assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private string GetAssemblyName()
        {
            return _assembly.GetName().Name;
        }
    }
}