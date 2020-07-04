using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System.Threading.Tasks;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderSetCollection)]
    public class BricksetApiServiceTestsSetCollection : BricksetApiServiceTestsBase
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
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            Check.ThatAsyncCode(() => _bricksetApiService.SetCollection(new SetCollectionParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public void InvalidUserHash()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidUserHash)));

            Check.ThatAsyncCode(() => _bricksetApiService.SetCollection(new SetCollectionParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public void InvalidParameters()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidParameters)));

            Check.ThatAsyncCode(() => _bricksetApiService.SetCollection(new SetCollectionParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public async Task Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var setCollectionResult = await _bricksetApiService.SetCollection(new SetCollectionParameters()).ConfigureAwait(false);

            Check.That(setCollectionResult).IsTrue();
        }
    }
}
