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
    [ComponentModelDescription(Constants.ApiResponseFolderGetAdditionalImages)]
    public class BricksetApiServiceTestsGetAdditionalImages : BricksetApiServiceTestsBase
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

            var additionalImages = _bricksetApiService.GetAdditionalImages(new ParameterSetId());

            additionalImages.Count().Should().Be(6);
        }

        [TestMethod]
        public void NoImages()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoImages)));

            var additionalImages = _bricksetApiService.GetAdditionalImages(new ParameterSetId());

            additionalImages.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidSetId()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidSetId)));

            var additionalImages = _bricksetApiService.GetAdditionalImages(new ParameterSetId());

            additionalImages.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var additionalImages = _bricksetApiService.GetAdditionalImages(new ParameterSetId());

            additionalImages.Should().BeEmpty();
        }
    }
}