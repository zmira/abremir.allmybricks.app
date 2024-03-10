using System.Threading.Tasks;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderCheckKey)]
    public class BricksetApiServiceTestsCheckKey : BricksetApiServiceTestsBase
    {
        private static BricksetApiService _bricksetApiService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _bricksetApiService = new BricksetApiService();
        }

        [TestMethod]
        public void Invalid()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Invalid)));

            Check.ThatCode(() => _bricksetApiService.CheckKey(new ParameterApiKey())).Throws<BricksetRequestException>();
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
