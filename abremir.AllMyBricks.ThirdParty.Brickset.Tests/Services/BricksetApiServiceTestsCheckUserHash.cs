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
    [ComponentModelDescription(Constants.ApiResponseFolderCheckUserHash)]
    public class BricksetApiServiceTestsCheckUserHash : BricksetApiServiceTestsBase
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
        public async Task ValidUserHash()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(ValidUserHash)));

            var userHashValidity = await _bricksetApiService.CheckUserHash(new ParameterUserHash());

            userHashValidity.Should()
                .NotBe(BricksetApiConstants.ResponseInvalid);
        }

        [TestMethod]
        public async Task InvalidUserHash()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidUserHash)));

            var userHashValidity = await _bricksetApiService.CheckUserHash(new ParameterUserHash());

            userHashValidity.Should()
                .Be(BricksetApiConstants.ResponseInvalid);
        }
    }
}
