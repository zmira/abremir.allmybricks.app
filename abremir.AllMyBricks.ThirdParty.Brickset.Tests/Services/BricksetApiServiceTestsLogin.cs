using System.Threading.Tasks;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using ComponentModelDescription = System.ComponentModel.DescriptionAttribute;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Tests.Services
{
    [TestClass]
    [ComponentModelDescription(Constants.ApiResponseFolderLogin)]
    public class BricksetApiServiceTestsLogin : BricksetApiServiceTestsBase
    {
        private static BricksetApiService _bricksetApiService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _bricksetApiService = new BricksetApiService();
        }

        [TestMethod]
        public void Invalid()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Invalid)));

            Check.ThatAsyncCode(() => _bricksetApiService.Login(new ParameterLogin())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            Check.ThatAsyncCode(() => _bricksetApiService.Login(new ParameterLogin())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public async Task Valid()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Valid)));

            var loginResult = await _bricksetApiService.Login(new ParameterLogin()).ConfigureAwait(false);

            Check.That(loginResult).Not.IsEmpty();
        }
    }
}
