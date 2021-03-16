﻿using abremir.AllMyBricks.ThirdParty.Brickset.Models;
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
    [ComponentModelDescription(Constants.ApiResponseFolderGetSets)]
    public class BricksetApiServiceTestsGetSets : BricksetApiServiceTestsBase
    {
        private static BricksetApiService _bricksetApiService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _bricksetApiService = new BricksetApiService();
        }

        [TestMethod]
        public void InvalidApiKey()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidApiKey)));

            Check.ThatAsyncCode(() => _bricksetApiService.GetSets(new GetSetsParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public void ParameterError()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(ParameterError)));

            Check.ThatAsyncCode(() => _bricksetApiService.GetSets(new GetSetsParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public void NoValidParameters()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(NoValidParameters)));

            Check.ThatAsyncCode(() => _bricksetApiService.GetSets(new GetSetsParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public void InvalidUserHash()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(InvalidUserHash)));

            Check.ThatAsyncCode(() => _bricksetApiService.GetSets(new GetSetsParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public void DailyApiLimitExceeded()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(DailyApiLimitExceeded)));

            Check.ThatAsyncCode(() => _bricksetApiService.GetSets(new GetSetsParameters())).Throws<BricksetRequestException>();
        }

        [TestMethod]
        public async Task Success()
        {
            _httpTestFake.RespondWith(GetResultFileFromResource(nameof(Success)));

            var sets = await _bricksetApiService.GetSets(new GetSetsParameters()).ConfigureAwait(false);

            Check.That(sets).CountIs(20);
        }
    }
}
