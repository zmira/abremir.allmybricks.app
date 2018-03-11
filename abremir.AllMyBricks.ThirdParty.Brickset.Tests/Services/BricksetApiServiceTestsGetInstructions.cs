using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderGetInstructions)]
    public class BricksetApiServiceTestsGetInstructions : BricksetApiServiceTestsBase
    {
        [TestMethod]
        public void Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var service = new BricksetApiService();

            var result = service.GetInstructions(new ParameterSetId());

            result.Should().NotBeEmpty();
        }

        [TestMethod]
        public void NoInstructions()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoInstructions)));

            var service = new BricksetApiService();

            var result = service.GetInstructions(new ParameterSetId());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidSetId()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidSetId)));

            var service = new BricksetApiService();

            var result = service.GetInstructions(new ParameterSetId());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var service = new BricksetApiService();

            var result = service.GetInstructions(new ParameterSetId());

            result.Should().BeEmpty();
        }
    }
}