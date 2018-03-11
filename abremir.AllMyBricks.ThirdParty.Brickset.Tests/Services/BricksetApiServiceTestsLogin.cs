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
    [ComponentModelDescription(Constants.ApiResponseFolderLogin)]
    public class BricksetApiServiceTestsLogin : BricksetApiServiceTestsBase
    {
        [TestMethod]
        public void ValidCredentials()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(ValidCredentials)));

            var service = new BricksetApiService();

            var result = service.Login(new ParameterLogin());

            result.Should()
                .NotBeNullOrEmpty()
                .And.NotStartWith(BricksetApiConstants.ResponseError)
                .And.NotStartWith(BricksetApiConstants.ResponseInvalidKey);
        }

        [TestMethod]
        public void InvalidCredentials()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidCredentials)));

            var service = new BricksetApiService();

            var result = service.Login(new ParameterLogin());

            result.Should()
                .NotBeNullOrEmpty()
                .And.StartWith(BricksetApiConstants.ResponseError)
                .And.NotStartWith(BricksetApiConstants.ResponseInvalidKey);
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var service = new BricksetApiService();

            var result = service.Login(new ParameterLogin());

            result.Should()
                .NotBeNullOrEmpty()
                .And.NotStartWith(BricksetApiConstants.ResponseError)
                .And.StartWith(BricksetApiConstants.ResponseInvalidKey);
        }
    }
}
