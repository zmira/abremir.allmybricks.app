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
    [ComponentModelDescription(Constants.ApiResponseFolderGetInstructions)]
    public class BricksetApiServiceTestsGetInstructions : BricksetApiServiceTestsBase
    {
        private static BricksetApiService _bricksetApiService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _bricksetApiService = new BricksetApiService();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            Check.ThatCode(() => _bricksetApiService.GetInstructions(new ParameterSetId())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public async Task NoMatches()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoMatches)));

            var instructions = await _bricksetApiService.GetInstructions(new ParameterSetId());

            Check.That(instructions).IsEmpty();
        }

        [TestMethod]
        public async Task Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var instructions = await _bricksetApiService.GetInstructions(new ParameterSetId());

            Check.That(instructions).CountIs(6);
        }
    }
}
