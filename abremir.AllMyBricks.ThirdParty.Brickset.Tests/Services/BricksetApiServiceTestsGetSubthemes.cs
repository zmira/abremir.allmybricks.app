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
    [ComponentModelDescription(Constants.ApiResponseFolderGetSubthemes)]
    public class BricksetApiServiceTestsGetSubthemes : BricksetApiServiceTestsBase
    {
        [TestMethod]
        public void Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var service = new BricksetApiService();

            var result = service.GetSubthemes(new ParameterTheme());

            result.Should().NotBeEmpty();
        }

        [TestMethod]
        public void NoSubthemes()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoSubthemes)));

            var service = new BricksetApiService();

            var result = service.GetSubthemes(new ParameterTheme());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidTheme()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidTheme)));

            var service = new BricksetApiService();

            var result = service.GetSubthemes(new ParameterTheme());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var service = new BricksetApiService();

            var result = service.GetSubthemes(new ParameterTheme());

            result.Should().BeEmpty();
        }
    }
}