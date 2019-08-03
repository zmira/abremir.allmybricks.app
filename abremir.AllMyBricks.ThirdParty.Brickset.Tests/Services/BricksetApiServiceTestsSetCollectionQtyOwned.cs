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
    [ComponentModelDescription(Constants.ApiResponseFolderSetCollectionOwns)]
    public class BricksetApiServiceTestsSetCollectionQtyOwned : BricksetApiServiceTestsBase
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
        public async Task InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            var setCollectionQtyOwnedResult = await _bricksetApiService.SetCollectionQtyOwned(new ParameterSetCollectionQtyOwned());

            setCollectionQtyOwnedResult.Should()
                .Be(BricksetApiConstants.ResponseInvalidKey);
        }

        [TestMethod]
        public async Task InvalidUserHash()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidUserHash)));

            var setCollectionQtyOwnedResult = await _bricksetApiService.SetCollectionQtyOwned(new ParameterSetCollectionQtyOwned());

            setCollectionQtyOwnedResult.Should()
                .StartWith(BricksetApiConstants.ResponseError);
        }

        [TestMethod]
        public async Task InvalidParameters()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidParameters)));

            var setCollectionQtyOwnedResult = await _bricksetApiService.SetCollectionQtyOwned(new ParameterSetCollectionQtyOwned());

            setCollectionQtyOwnedResult.Should()
                .StartWith(BricksetApiConstants.ResponseError);
        }

        [TestMethod]
        public async Task Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var setCollectionQtyOwnedResult = await _bricksetApiService.SetCollectionQtyOwned(new ParameterSetCollectionQtyOwned());

            setCollectionQtyOwnedResult.Should()
                .Be(BricksetApiConstants.ResponseOk);
        }
    }
}
