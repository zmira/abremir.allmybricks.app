using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class ThemeRepositoryTests : TestRepositoryBase
    {
        private static IThemeRepository _themeRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RCS1163 // Unused parameter.
        {
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void GivenGetTheme_WhenThemeNameNotValid_ThenReturnsNull(string themeName)
        {
            var theme = _themeRepository.GetTheme(themeName);

            theme.Should().BeNull();
        }

        [TestMethod]
        public void GivenGetTheme_WhenThemeDoesNotExist_ThenReturnsNull()
        {
            InsertData(ModelsSetup.ThemeUnderTest);

            var theme = _themeRepository.GetTheme(ModelsSetup.NonExistentThemeName);

            theme.Should().BeNull();
        }

        [TestMethod]
        public void GivenGetTheme_WhenThemeExists_ThenReturnsModel()
        {
            InsertData(ModelsSetup.ThemeUnderTest);

            var theme = _themeRepository.GetTheme(ModelsSetup.ThemeUnderTestName);

            theme.Should().BeEquivalentTo(ModelsSetup.ThemeUnderTest);
        }

        [TestMethod]
        public void GivenGetAllThemes_WhenNoThemes_ThenReturnsEmpty()
        {
            var allThemes = _themeRepository.GetAllThemes();

            allThemes.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllThemes_WhenHasThemes_ThenReturnsModels()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemes = _themeRepository.GetAllThemes();

            allThemes.Should().BeEquivalentTo(ModelsSetup.ListOfThemesUnderTest);
        }

        [TestMethod]
        public void GivenGetAllThemesForYear_WhenYearIsLessThanMinimumConstant_ThenReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.GetAllThemesForYear(Constants.MinimumSetYear - 1);

            allThemesForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllThemesForYear_WhenNoThemesForYear_ThenReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.GetAllThemesForYear(ModelsSetup.SecondThemeYearTo + 1);

            allThemesForYear.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.FirstThemeYearTo, 2)]
        [DataRow(ModelsSetup.SecondThemeYearTo, 1)]
        public void GivenGetAllThemesForYear_WhenHasThemesForYear_ThenReturnsModels(ushort year, int expectedCount)
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.GetAllThemesForYear(year);

            allThemesForYear.Should().HaveCount(expectedCount);
        }

        [TestMethod]
        public void GivenAddOrUpdateTheme_WhenNullTheme_ThenReturnsNull()
        {
            var theme = _themeRepository.AddOrUpdateTheme(null);

            theme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void GivenAddOrUpdateTheme_WhenInvalidTheme_ThenReturnsNull(string themeName)
        {
            var theme = _themeRepository.AddOrUpdateTheme(new Theme { Name = themeName });

            theme.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateTheme_WhenThemeYearFromIsLessThanMinimumConstant_ThenReturnsNull()
        {
            var themeUnderTest = ModelsSetup.ThemeUnderTest;
            themeUnderTest.YearFrom = Constants.MinimumSetYear - 1;

            var theme = _themeRepository.AddOrUpdateTheme(themeUnderTest);

            theme.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateTheme_WhenNewValidTheme_ThenInsertsModel()
        {
            _themeRepository.AddOrUpdateTheme(ModelsSetup.ThemeUnderTest);

            var theme = _themeRepository.GetTheme(ModelsSetup.ThemeUnderTest.Name);

            theme.Should().BeEquivalentTo(ModelsSetup.ThemeUnderTest);
        }

        [TestMethod]
        public void GivenAddOrUpdateTheme_WhenExistingValidTheme_ThenUpdatesModel()
        {
            _themeRepository.AddOrUpdateTheme(ModelsSetup.ThemeUnderTest);

            var themeUnderTest = _themeRepository.GetTheme(ModelsSetup.ThemeUnderTest.Name);

            themeUnderTest.SetCount = 99;
            themeUnderTest.YearTo = 2099;

            _themeRepository.AddOrUpdateTheme(themeUnderTest);

            var theme = _themeRepository.GetTheme(themeUnderTest.Name);

            theme.Should().BeEquivalentTo(themeUnderTest);
        }
    }
}