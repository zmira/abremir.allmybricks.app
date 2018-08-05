using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
        public void Get_ThemeNameNotValid_ReturnsNull(string themeName)
        {
            var theme = _themeRepository.Get(themeName);

            theme.Should().BeNull();
        }

        [TestMethod]
        public void Get_ThemeDoesNotExist_ReturnsNull()
        {
            InsertData(ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString()));

            var theme = _themeRepository.Get(ModelsSetup.NonExistentThemeName);

            theme.Should().BeNull();
        }

        [TestMethod]
        public void Get_ThemeExists_ReturnsModel()
        {
            var themeUnderTest = InsertData(ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString()));

            var theme = _themeRepository.Get(themeUnderTest.Name);

            theme.Name.Should().BeEquivalentTo(themeUnderTest.Name);
        }

        [TestMethod]
        public void All_NoThemes_ReturnsEmpty()
        {
            var allThemes = _themeRepository.All();

            allThemes.Should().BeEmpty();
        }

        [TestMethod]
        public void All_HasThemes_ReturnsModels()
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemes = _themeRepository.All();

            allThemes.Select(theme => theme.Name).Should().BeEquivalentTo(listOfThemesUnderTest.Select(theme => theme.Name));
        }

        [TestMethod]
        public void AllForYear_YearIsLessThanMinimumConstant_ReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.AllForYear(Constants.MinimumSetYear - 1);

            allThemesForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForYear_NoThemesForYear_ReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.AllForYear(ModelsSetup.SecondThemeYearTo + 1);

            allThemesForYear.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.FirstThemeYearTo, 2)]
        [DataRow(ModelsSetup.SecondThemeYearTo, 1)]
        public void AllForYear_HasThemesForYear_ReturnsModels(short year, int expectedCount)
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.AllForYear(year).ToList();

            allThemesForYear.Should().HaveCount(expectedCount);
        }

        [TestMethod]
        public void AddOrUpdate_NullTheme_ReturnsNull()
        {
            var theme = _themeRepository.AddOrUpdate(null);

            theme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AddOrUpdate_InvalidTheme_ReturnsNull(string themeName)
        {
            var theme = _themeRepository.AddOrUpdate(new Theme { Name = themeName });

            theme.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_ThemeYearFromIsLessThanMinimumConstant_ReturnsNull()
        {
            var themeUnderTest = ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString());
            themeUnderTest.YearFrom = Constants.MinimumSetYear - 1;

            var theme = _themeRepository.AddOrUpdate(themeUnderTest.ToPlainObject());

            theme.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_NewValidTheme_InsertsModel()
        {
            var themeUnderTest = ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString());

            _themeRepository.AddOrUpdate(themeUnderTest.ToPlainObject());

            var theme = _themeRepository.Get(themeUnderTest.Name);

            theme.Name.Should().BeEquivalentTo(themeUnderTest.Name);
        }

        [TestMethod]
        public void AddOrUpdate_ExistingValidTheme_UpdatesModel()
        {
            var themeUnderTest = ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString());

            _themeRepository.AddOrUpdate(themeUnderTest.ToPlainObject());

            var result = _themeRepository.Get(themeUnderTest.Name);

            result.SetCount = 99;
            result.YearTo = 2099;

            _themeRepository.AddOrUpdate(result);

            var theme = _themeRepository.Get(result.Name);

            theme.Should().BeEquivalentTo(result);
        }
    }
}