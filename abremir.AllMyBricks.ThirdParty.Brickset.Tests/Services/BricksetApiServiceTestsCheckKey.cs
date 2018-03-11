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
    [ComponentModelDescription(Constants.ApiResponseFolderCheckKey)]
    public class BricksetApiServiceTestsCheckKey : BricksetApiServiceTestsBase
    {
        [TestMethod]
        public void ValidKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(ValidKey)));

            var service = new BricksetApiService();

            var result = service.CheckKey(new ParameterApiKey());

            result.Should().BeTrue();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var service = new BricksetApiService();

            var result = service.CheckKey(new ParameterApiKey());

            result.Should().BeFalse();
        }
    }
}
