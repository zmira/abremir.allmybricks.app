using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderGetInstructions)]
    public class BricksetApiServiceTestsGetInstructions : BricksetApiServiceTestsBase
    {
        private static BricksetApiService _bricksetApiService;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _bricksetApiService = new BricksetApiService();
        }

        [TestMethod]
        public void Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var instructions = _bricksetApiService.GetInstructions(new ParameterSetId());

            instructions.Count().Should().Be(2);
        }

        [TestMethod]
        public void NoInstructions()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoInstructions)));

            var instructions = _bricksetApiService.GetInstructions(new ParameterSetId());

            instructions.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidSetId()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidSetId)));

            var instructions = _bricksetApiService.GetInstructions(new ParameterSetId());

            instructions.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var instructions = _bricksetApiService.GetInstructions(new ParameterSetId());

            instructions.Should().BeEmpty();
        }
    }
}