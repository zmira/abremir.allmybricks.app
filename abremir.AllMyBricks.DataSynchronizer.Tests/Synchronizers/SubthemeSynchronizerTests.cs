using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using Easy.MessageHub;
using fastJSON;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public static void ClassInitialize(TestContext testContext)
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

            var subthemes = await subthemeSynchronizer.Synchronize(string.Empty, new Theme { Name = string.Empty });

            subthemes.Should().BeEmpty();
            _subthemeRepository.All().Should().BeEmpty();
        }

        [TestMethod]
        public async Task Synchronize_BricksetApiServiceReturnsListOfSubthemes_AllSubthemesAreSaved()
        {
            var testTheme = JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes)).First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var yearsList = JSON.ToObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));
            var subthemesList = JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));

            var theme = testTheme.ToTheme();
            theme.SetCountPerYear = yearsList.ToYearSetCountEnumerable().ToList();

            _themeRepository.AddOrUpdate(theme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSubthemes(Arg.Any<ParameterTheme>())
                .Returns(subthemesList);

            var subthemeSynchronizer = CreateTarget(bricksetApiService);

            var subthemes = await subthemeSynchronizer.Synchronize(string.Empty, theme);

            subthemes.Count().Should().Be(subthemesList.Count);
            _subthemeRepository.All().Count().Should().Be(subthemesList.Count);
        }

        private SubthemeSynchronizer CreateTarget(IBricksetApiService bricksetApiService = null)
        {
            bricksetApiService = bricksetApiService ?? Substitute.For<IBricksetApiService>();

            return new SubthemeSynchronizer(bricksetApiService, _subthemeRepository, Substitute.For<IMessageHub>());
        }
    }
}
