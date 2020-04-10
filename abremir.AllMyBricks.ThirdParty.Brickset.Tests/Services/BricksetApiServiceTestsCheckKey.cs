using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System.Threading.Tasks;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderCheckKey)]
    public class BricksetApiServiceTestsCheckKey : BricksetApiServiceTestsBase
    {
        private static BricksetApiService _bricksetApiService;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
#pragma warning disable IDE0060 // Remove unused parameter
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _bricksetApiService = new BricksetApiService();
        }

        [TestMethod]
        public async Task Invalid()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Invalid)));

            var keyValidity = await _bricksetApiService.CheckKey(new ParameterApiKey());

            Check.That(keyValidity).IsFalse();
        }

        [TestMethod]
        public async Task Valid()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Valid)));

            var keyValidity = await _bricksetApiService.CheckKey(new ParameterApiKey());

            Check.That(keyValidity).IsTrue();
        }
    }
}
