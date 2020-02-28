using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using abremir.AllMyBricks.Data.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class SubthemeRepositoryTests : DataTestsBase
    {
        private static ISubthemeRepository _subthemeRepository;

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
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(ModelsSetup.StringEmpty, null)]
        [DataRow(null, ModelsSetup.StringEmpty)]
        [DataRow(ModelsSetup.StringEmpty, ModelsSetup.StringEmpty)]
        public void Get_InvalidParameters_ReturnsNull(string themeName, string subthemeName)
        {
            var subtheme = _subthemeRepository.Get(themeName, subthemeName);

            subtheme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.ThemeUnderTestName, ModelsSetup.NonExistentSubthemeName)]
        [DataRow(ModelsSetup.NonExistentThemeName, ModelsSetup.SubthemeUnderTestName)]
        public void Get_SubthemeDoesNotExist_ReturnsNull(string themeName, string subthemeName)
        {
            var subthemeUnderTest = ModelsSetup.GetSubthemeUnderTest(subthemeName == ModelsSetup.SubthemeUnderTestName ? ModelsSetup.SubthemeUnderTestName : Guid.NewGuid().ToString());
            subthemeUnderTest.Theme = InsertData(ModelsSetup.GetThemeUnderTest(themeName == ModelsSetup.ThemeUnderTestName ? themeName : Guid.NewGuid().ToString()));

            InsertData(subthemeUnderTest);

            var subtheme = _subthemeRepository.Get(themeName, subthemeName);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void Get_SubthemeExists_ReturnModel()
        {
            var subthemeUnderTest = ModelsSetup.GetSubthemeUnderTest(Guid.NewGuid().ToString());

            InsertData(subthemeUnderTest.Theme);
            InsertData(subthemeUnderTest);

            var subtheme = _subthemeRepository.Get(subthemeUnderTest.Theme.Name, subthemeUnderTest.Name);

            subtheme.Name.Should().BeEquivalentTo(subthemeUnderTest.Name);
        }

        [TestMethod]
        public void All_NoSubthemes_ReturnsEmpty()
        {
            var allSubthemes = _subthemeRepository.All();

            allSubthemes.Should().BeEmpty();
        }

        [TestMethod]
        public void All_HasSubthemes_ReturnsModels()
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemesUnderTest = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemesUnderTest[0].Theme = listOfThemesUnderTest[0];
            listOfSubthemesUnderTest[1].Theme = listOfThemesUnderTest[0];

            InsertData(listOfSubthemesUnderTest);

            var allSubthemes = _subthemeRepository.All();

            allSubthemes.Select(subtheme => subtheme.Name).Should().BeEquivalentTo(listOfSubthemesUnderTest.Select(subtheme => subtheme.Name));
        }

        [TestMethod]
        public void AllForYear_NoSubthemesForYear_ReturnsEmpty()
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemesUnderTest = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemesUnderTest[0].Theme = listOfThemesUnderTest[0];
            listOfSubthemesUnderTest[1].Theme = listOfThemesUnderTest[0];

            InsertData(listOfSubthemesUnderTest);

            var allSubthemesForYear = _subthemeRepository.AllForYear(ModelsSetup.FirstThemeYearTo + 1);

            allSubthemesForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForYear_YearIsLessThanMinimumConstant_ReturnsEmpty()
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemesUnderTest = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemesUnderTest[0].Theme = listOfThemesUnderTest[0];
            listOfSubthemesUnderTest[1].Theme = listOfThemesUnderTest[0];

            InsertData(listOfSubthemesUnderTest);

            var allSubthemesForYear = _subthemeRepository.AllForYear(Constants.MinimumSetYear - 1);

            allSubthemesForYear.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.FirstSubthemeYearFrom, 1)]
        [DataRow(ModelsSetup.SecondSubthemeYearFrom, 2)]
        public void AllForYear_HasSubthemesForYear_ReturnsModels(short year, int expectedCount)
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemesUnderTest = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemesUnderTest[0].Theme = listOfThemesUnderTest[0];
            listOfSubthemesUnderTest[1].Theme = listOfThemesUnderTest[0];

            InsertData(listOfSubthemesUnderTest);

            var allSubthemesForYear = _subthemeRepository.AllForYear(year);

            allSubthemesForYear.Should().HaveCount(expectedCount);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForTheme_InvalidThemeName_ReturnsEmpty(string themeName)
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemesUnderTest = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemesUnderTest[0].Theme = listOfThemesUnderTest[0];
            listOfSubthemesUnderTest[1].Theme = listOfThemesUnderTest[0];

            InsertData(listOfSubthemesUnderTest);

            var allSubthemesForTheme = _subthemeRepository.AllForTheme(themeName);

            allSubthemesForTheme.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForTheme_NoSubthemesForTheme_ReturnsEmpty()
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemesUnderTest = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemesUnderTest[0].Theme = listOfThemesUnderTest[0];
            listOfSubthemesUnderTest[1].Theme = listOfThemesUnderTest[0];

            InsertData(listOfSubthemesUnderTest);

            var allSubthemesForTheme = _subthemeRepository.AllForTheme(ModelsSetup.NonExistentThemeName);

            allSubthemesForTheme.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForTheme_HasSubthemesForTheme_ReturnsModels()
        {
            var listOfThemesUnderTest = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemesUnderTest = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemesUnderTest[0].Theme = listOfThemesUnderTest[0];
            listOfSubthemesUnderTest[1].Theme = listOfThemesUnderTest[0];

            InsertData(listOfSubthemesUnderTest);

            var allSubthemesForTheme = _subthemeRepository.AllForTheme(listOfThemesUnderTest[0].Name);

            allSubthemesForTheme.Should().HaveCount(listOfSubthemesUnderTest.Length);
        }

        [TestMethod]
        public void AddOrUpdate_NullSubtheme_ReturnsNull()
        {
            Subtheme subthemeUnderTest = null;

            var subtheme = _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AddOrUpdate_InvalidSubtheme_ReturnsNull(string subthemeName)
        {
            var subthemeUnderTest = new Subtheme { Name = subthemeName };

            var subtheme = _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_NullTheme_ReturnsNull()
        {
            var subthemeUnderTest = new Subtheme { Name = ModelsSetup.NonExistentSubthemeName };

            var subtheme = _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AddOrUpdate_InvalidTheme_ReturnsNull(string themeName)
        {
            var subthemeUnderTest = new Subtheme { Name = ModelsSetup.NonExistentSubthemeName, Theme = new Theme { Name = themeName } };

            var subtheme = _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_SubthemeYearFromIsLessThanMinimumConstant_ReturnsNull()
        {
            var subthemeUnderTest = ModelsSetup.GetSubthemeUnderTest(Guid.NewGuid().ToString());
            subthemeUnderTest.YearFrom = Constants.MinimumSetYear - 1;

            var subtheme = _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_ThemeYearFromIsLessThanMinimumConstant_ReturnsNull()
        {
            var subthemeUnderTest = ModelsSetup.GetSubthemeUnderTest(Guid.NewGuid().ToString());
            subthemeUnderTest.Theme.YearFrom = Constants.MinimumSetYear - 1;

            var subtheme = _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_NewValidSubtheme_InsertsModel()
        {
            var subthemeUnderTest = ModelsSetup.GetSubthemeUnderTest(Guid.NewGuid().ToString());

            InsertData(subthemeUnderTest.Theme);

            _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            var subtheme = _subthemeRepository.Get(subthemeUnderTest.Theme.Name, subthemeUnderTest.Name);

            subtheme.Name.Should().BeEquivalentTo(subthemeUnderTest.Name);
        }

        [TestMethod]
        public void AddOrUpdate_ExistingValidSubtheme_UpdatesModel()
        {
            var subtheme = ModelsSetup.GetSubthemeUnderTest(Guid.NewGuid().ToString());

            InsertData(subtheme.Theme);

            _subthemeRepository.AddOrUpdate(subtheme);

            var subthemeUnderTest = _subthemeRepository.Get(subtheme.Theme.Name, subtheme.Name);

            subthemeUnderTest.SetCount = 66;
            subthemeUnderTest.YearTo = 2099;

            _subthemeRepository.AddOrUpdate(subthemeUnderTest);

            var saveSubtheme = _subthemeRepository.Get(subtheme.Theme.Name, subtheme.Name);

            saveSubtheme.SetCount.Should().Be(subthemeUnderTest.SetCount);
            saveSubtheme.YearTo.Should().Be(subthemeUnderTest.YearTo);
        }
    }
}
