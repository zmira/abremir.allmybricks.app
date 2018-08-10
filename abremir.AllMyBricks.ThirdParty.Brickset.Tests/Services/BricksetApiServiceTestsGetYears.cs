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
    [ComponentModelDescription(Constants.ApiResponseFolderGetYears)]
    public class BricksetApiServiceTestsGetYears : BricksetApiServiceTestsBase
    {
        [TestMethod]
        public void Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var service = new BricksetApiService();

            var result = service.GetYears(new ParameterTheme());

            result.Count().Should().Be(11);
        }

        [TestMethod]
        public void NoYears()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoYears)));

            var service = new BricksetApiService();

            var result = service.GetYears(new ParameterTheme());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidTheme()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidTheme)));

            var service = new BricksetApiService();

            var result = service.GetYears(new ParameterTheme());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var service = new BricksetApiService();

            var result = service.GetYears(new ParameterTheme());

            result.Should().BeEmpty();
        }
    }
}
