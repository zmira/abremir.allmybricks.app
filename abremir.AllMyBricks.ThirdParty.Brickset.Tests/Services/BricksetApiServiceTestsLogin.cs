using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using BricksetApiConstants = abremir.AllMyBricks.ThirdParty.Brickset.Configuration.Constants;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderLogin)]
    public class BricksetApiServiceTestsLogin : BricksetApiServiceTestsBase
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
        public async Task ValidCredentials()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(ValidCredentials)));

            var loginResult = await _bricksetApiService.Login(new ParameterLogin());

            loginResult.Should()
                .NotStartWith(BricksetApiConstants.ResponseError)
                .And.NotBe(BricksetApiConstants.ResponseInvalidKey);
        }

        [TestMethod]
        public async Task InvalidCredentials()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidCredentials)));

            var loginResult = await _bricksetApiService.Login(new ParameterLogin());

            loginResult.Should()
                .StartWith(BricksetApiConstants.ResponseError);
        }

        [TestMethod]
        public async Task InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var loginResult = await _bricksetApiService.Login(new ParameterLogin());

            loginResult.Should()
                .Be(BricksetApiConstants.ResponseInvalidKey);
        }
    }
}
