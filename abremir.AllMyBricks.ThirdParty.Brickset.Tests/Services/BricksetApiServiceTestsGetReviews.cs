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
    [ComponentModelDescription(Constants.ApiResponseFolderGetReviews)]
    public class BricksetApiServiceTestsGetReviews : BricksetApiServiceTestsBase
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
        public async Task Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var reviews = await _bricksetApiService.GetReviews(new ParameterSetId());

            reviews.Count().Should()
                .Be(5);
        }

        [TestMethod]
        public async Task NoReviews()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoReviews)));

            var reviews = await _bricksetApiService.GetReviews(new ParameterSetId());

            reviews.Should()
                .BeEmpty();
        }

        [TestMethod]
        public async Task InvalidSetId()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidSetId)));

            var reviews = await _bricksetApiService.GetReviews(new ParameterSetId());

            reviews.Should()
                .BeEmpty();
        }

        [TestMethod]
        public async Task InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var reviews = await _bricksetApiService.GetReviews(new ParameterSetId());

            reviews.Should()
                .BeEmpty();
        }
    }
}
