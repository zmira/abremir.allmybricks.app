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
        [TestMethod]
        public void Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var service = new BricksetApiService();

            var result = service.GetAdditionalImages(new ParameterSetId());

            result.Count().Should().Be(6);
        }

        [TestMethod]
        public void NoImages()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoImages)));

            var service = new BricksetApiService();

            var result = service.GetAdditionalImages(new ParameterSetId());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidSetId()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidSetId)));

            var service = new BricksetApiService();

            var result = service.GetAdditionalImages(new ParameterSetId());

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var service = new BricksetApiService();

            var result = service.GetAdditionalImages(new ParameterSetId());

            result.Should().BeEmpty();
        }
    }
}