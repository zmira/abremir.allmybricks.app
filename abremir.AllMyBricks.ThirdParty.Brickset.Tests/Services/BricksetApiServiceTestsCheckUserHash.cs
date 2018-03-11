using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BricksetApiConstants = abremir.AllMyBricks.ThirdParty.Brickset.Configuration.Constants;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderCheckUserHash)]
    public class BricksetApiServiceTestsCheckUserHash : BricksetApiServiceTestsBase
    {
        [TestMethod]
        public void ValidUserHash()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(ValidUserHash)));

            var service = new BricksetApiService();

            var result = service.CheckUserHash(new ParameterUserHash());

            result.Should()
                .NotBeNullOrEmpty()
                .And.NotBe(BricksetApiConstants.ResponseInvalid);
        }

        [TestMethod]
        public void InvalidUserHash()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidUserHash)));

            var service = new BricksetApiService();

            var result = service.CheckUserHash(new ParameterUserHash());

            result.Should()
                .NotBeNullOrEmpty()
                .And.Be(BricksetApiConstants.ResponseInvalid);
        }
    }
}
