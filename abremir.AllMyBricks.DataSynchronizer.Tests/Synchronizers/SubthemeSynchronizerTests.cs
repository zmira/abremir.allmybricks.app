using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
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
    public class SubthemeSynchronizerTests : DataSynchronizerTestsBase
    {
        private static ISubthemeRepository _subthemeRepository;
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
            _subthemeRepository = new SubthemeRepository(MemoryRepositoryService);
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public async Task Synchronize_BricksetApiServiceReturnsEmptyList_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSubthemes(Arg.Any<ParameterTheme>())
                .Returns(Enumerable.Empty<Subthemes>());

            var subthemeSynchronizer = CreateTarget(bricksetApiService);

            var subthemes = await subthemeSynchronizer.Synchronize(string.Empty, new Theme { Name = string.Empty }).ConfigureAwait(false);

            Check.That(subthemes).IsEmpty();
            Check.That(_subthemeRepository.All()).IsEmpty();
        }

        [TestMethod]
        public async Task Synchronize_BricksetApiServiceReturnsListOfSubthemes_AllSubthemesAreSaved()
        {
            var testTheme = JsonConvert.DeserializeObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes)).First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var yearsList = JsonConvert.DeserializeObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));
            var subthemesList = JsonConvert.DeserializeObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));

            var theme = testTheme.ToTheme();
            theme.SetCountPerYear = yearsList.ToYearSetCountEnumerable().ToList();

            _themeRepository.AddOrUpdate(theme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSubthemes(Arg.Any<ParameterTheme>())
                .Returns(subthemesList);

            var subthemeSynchronizer = CreateTarget(bricksetApiService);

            var subthemes = await subthemeSynchronizer.Synchronize(string.Empty, theme).ConfigureAwait(false);

            Check.That(subthemes).CountIs(subthemesList.Count);
            Check.That(_subthemeRepository.All()).CountIs(subthemesList.Count);
        }

        private SubthemeSynchronizer CreateTarget(IBricksetApiService bricksetApiService = null)
        {
            bricksetApiService ??= Substitute.For<IBricksetApiService>();

            return new SubthemeSynchronizer(bricksetApiService, _subthemeRepository, Substitute.For<IMessageHub>());
        }
    }
}
