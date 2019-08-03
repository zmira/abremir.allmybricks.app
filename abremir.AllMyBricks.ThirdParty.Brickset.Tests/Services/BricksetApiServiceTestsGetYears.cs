using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderGetYears)]
    public class BricksetApiServiceTestsGetYears : BricksetApiServiceTestsBase
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
        public async Task Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var years = await _bricksetApiService.GetYears(new ParameterTheme());

            years.Count().Should()
                .Be(11);
        }

        [TestMethod]
        public async Task NoYears()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoYears)));

            var years = await _bricksetApiService.GetYears(new ParameterTheme());

            years.Should()
                .BeEmpty();
        }

        [TestMethod]
        public async Task InvalidTheme()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidTheme)));

            var years = await _bricksetApiService.GetYears(new ParameterTheme());

            years.Should()
                .BeEmpty();
        }

        [TestMethod]
        public async Task InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var years = await _bricksetApiService.GetYears(new ParameterTheme());

            years.Should()
                .BeEmpty();
        }
    }
}
