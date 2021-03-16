using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using abremir.AllMyBricks.Data.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Linq;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class ThemeRepositoryTests : DataTestsBase
    {
        private static IThemeRepository _themeRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Get_InvalidThemeName_ReturnsNull(string themeName)
        {
            var theme = _themeRepository.Get(themeName);

            Check.That(theme).IsNull();
        }

        [TestMethod]
        public void Get_ThemeDoesNotExist_ReturnsNull()
        {
            InsertData(ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString()));

            var theme = _themeRepository.Get(ModelsSetup.NonExistentThemeName);

            Check.That(theme).IsNull();
        }

        [TestMethod]
        public void Get_ThemeExists_ReturnsModel()
        {
            var themeUnderTest = InsertData(ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString()));

            var theme = _themeRepository.Get(themeUnderTest.Name);

            Check.That(theme.Name).IsEqualTo(themeUnderTest.Name);
        }

        [TestMethod]
        public void All_NoThemes_ReturnsEmpty()
        {
            var allThemes = _themeRepository.All();

            Check.That(allThemes).IsEmpty();
        }

        [TestMethod]
        public void All_HasThemes_ReturnsModels()
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemes = _themeRepository.All();

            Check.That(allThemes.Select(theme => theme.Name)).IsEquivalentTo(listOfThemesUnderTest.Select(theme => theme.Name));
        }

        [TestMethod]
        public void AllForYear_YearIsLessThanMinimumConstant_ReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.AllForYear(Constants.MinimumSetYear - 1);

            Check.That(allThemesForYear).IsEmpty();
        }

        [TestMethod]
        public void AllForYear_NoThemesForYear_ReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.AllForYear(ModelsSetup.SecondThemeYearTo + 1);

            Check.That(allThemesForYear).IsEmpty();
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.FirstThemeYearTo, 2)]
        [DataRow(ModelsSetup.SecondThemeYearTo, 1)]
        public void AllForYear_HasThemesForYear_ReturnsModels(short year, int expectedCount)
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);

            var allThemesForYear = _themeRepository.AllForYear(year).ToList();

            Check.That(allThemesForYear).CountIs(expectedCount);
        }

        [TestMethod]
        public void AddOrUpdate_NullTheme_ReturnsNull()
        {
            var theme = _themeRepository.AddOrUpdate(null);

            Check.That(theme).IsNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AddOrUpdate_InvalidTheme_ReturnsNull(string themeName)
        {
            var theme = _themeRepository.AddOrUpdate(new Theme { Name = themeName });

            Check.That(theme).IsNull();
        }

        [TestMethod]
        public void AddOrUpdate_ThemeYearFromIsLessThanMinimumConstant_ReturnsNull()
        {
            var themeUnderTest = ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString());
            themeUnderTest.YearFrom = Constants.MinimumSetYear - 1;

            var theme = _themeRepository.AddOrUpdate(themeUnderTest);

            Check.That(theme).IsNull();
        }

        [TestMethod]
        public void AddOrUpdate_NewValidTheme_InsertsModel()
        {
            var themeUnderTest = ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString());

            _themeRepository.AddOrUpdate(themeUnderTest);

            var theme = _themeRepository.Get(themeUnderTest.Name);

            Check.That(theme.Name).IsEqualTo(themeUnderTest.Name);
        }

        [TestMethod]
        public void AddOrUpdate_ExistingValidTheme_UpdatesModel()
        {
            var themeUnderTest = ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString());

            _themeRepository.AddOrUpdate(themeUnderTest);

            var themeFromDb = _themeRepository.Get(themeUnderTest.Name);

            themeFromDb.SetCount = 99;
            themeFromDb.YearTo = 2099;

            _themeRepository.AddOrUpdate(themeFromDb);

            var theme = _themeRepository.Get(themeFromDb.Name);

            Check.That(theme).HasFieldsWithSameValues(themeFromDb);
        }
    }
}
