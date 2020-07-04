using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NFluent;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Synchronizers
{
    [TestClass]
    public class ThemeSynchronizerTests : DataSynchronizerTestsBase
    {
        private static IThemeRepository _themeRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
#pragma warning disable IDE0060 // Remove unused parameter
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public async Task Synchronize_BricksetApiServiceReturnsEmptyList_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetThemes(Arg.Any<ParameterApiKey>())
                .Returns(Enumerable.Empty<Themes>());

            var themeSynchronizer = CreateTarget(bricksetApiService);

            var themes = await themeSynchronizer.Synchronize(string.Empty).ConfigureAwait(false);

            Check.That(themes).IsEmpty();
            Check.That(_themeRepository.All()).IsEmpty();
        }

        [TestMethod]
        public async Task Synchronize_BricksetApiServiceReturnsListOfThemes_AllThemesAreSaved()
        {
            var themesList = JsonConvert.DeserializeObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var yearsList = JsonConvert.DeserializeObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetThemes(Arg.Any<ParameterApiKey>())
                .Returns(themesList);
            bricksetApiService
                .GetYears(Arg.Is<ParameterTheme>(parameter => parameter.Theme == Constants.TestThemeArchitecture))
                .Returns(yearsList);

            var themeSynchronizer = CreateTarget(bricksetApiService);

            var themes = await themeSynchronizer.Synchronize(string.Empty).ConfigureAwait(false);

            Check.That(themes).CountIs(themesList.Count);
            Check.That(_themeRepository.All()).CountIs(themesList.Count);
            Check.That(_themeRepository.Get(Constants.TestThemeArchitecture).SetCountPerYear).Not.IsEmpty();
        }

        private ThemeSynchronizer CreateTarget(IBricksetApiService bricksetApiService = null)
        {
            bricksetApiService ??= Substitute.For<IBricksetApiService>();

            return new ThemeSynchronizer(bricksetApiService, _themeRepository, Substitute.For<IMessageHub>());
        }
    }
}
