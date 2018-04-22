using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class SubthemeRepositoryTests : TestRepositoryBase
    {
        private static ISubthemeRepository _subthemeRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RCS1163 // Unused parameter.
        {
            _subthemeRepository = new SubthemeRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(ModelsSetup.StringEmpty, null)]
        [DataRow(null, ModelsSetup.StringEmpty)]
        [DataRow(ModelsSetup.StringEmpty, ModelsSetup.StringEmpty)]
        public void GivenGetSubtheme_WhenInvalidParameters_ThenReturnsNull(string themeName, string subthemeName)
        {
            var subtheme = _subthemeRepository.GetSubtheme(themeName, subthemeName);

            subtheme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.ThemeUnderTestName, ModelsSetup.NonExistentSubthemeName)]
        [DataRow(ModelsSetup.NonExistentThemeName, ModelsSetup.SubthemeUnderTestName)]
        public void GivenGetSubtheme_WhenSubthemeDoesNotExist_ThenReturnsNull(string themeName, string subthemeName)
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.SubthemeUnderTest);

            var subtheme = _subthemeRepository.GetSubtheme(themeName, subthemeName);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void GivenGetSubtheme_WhenSubthemeExists_ThenReturnModel()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.SubthemeUnderTest);

            var subtheme = _subthemeRepository.GetSubtheme(ModelsSetup.ThemeUnderTestName, ModelsSetup.SubthemeUnderTestName);

            subtheme.Should().BeEquivalentTo(ModelsSetup.SubthemeUnderTest);
        }

        [TestMethod]
        public void GivenGetAllSubthemes_WhenNoSubthemes_ThenReturnsEmpty()
        {
            var allSubthemes = _subthemeRepository.GetAllSubthemes();

            allSubthemes.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllSubthemes_WhenHasSubthemes_ThenReturnsModels()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);
            InsertData(ModelsSetup.ListOfSubthemesUnderTest);

            var allSubthemes = _subthemeRepository.GetAllSubthemes();

            allSubthemes.Should().BeEquivalentTo(ModelsSetup.ListOfSubthemesUnderTest);
        }

        [TestMethod]
        public void GivenGetAllSubthemesForYear_WhenNoSubthemesForYear_ThenReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);
            InsertData(ModelsSetup.ListOfSubthemesUnderTest);

            var allSubthemesForYear = _subthemeRepository.GetAllSubthemesForYear(ModelsSetup.FirstThemeYearTo + 1);

            allSubthemesForYear.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.FirstSubthemeYearFrom, 1)]
        [DataRow(ModelsSetup.SecondSubthemeYearFrom, 2)]
        public void GivenGetAllSubthemesForYear_WhenHasSubthemesForYear_ThenReturnsModels(ushort year, int expectedCount)
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);
            InsertData(ModelsSetup.ListOfSubthemesUnderTest);

            var allSubthemesForYear = _subthemeRepository.GetAllSubthemesForYear(year);

            allSubthemesForYear.Should().HaveCount(expectedCount);
        }

        [TestMethod]
        public void GivenGetAllSubthemesForTheme_WhenNoSubthemesForTheme_ThenReturnsEmpty()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);
            InsertData(ModelsSetup.ListOfSubthemesUnderTest);

            var allSubthemesForTheme = _subthemeRepository.GetAllSubthemesForTheme(ModelsSetup.NonExistentThemeName);

            allSubthemesForTheme.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllSubthemesForTheme_WhenHasSubthemesForTheme_ThenReturnsModels()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);
            InsertData(ModelsSetup.ListOfSubthemesUnderTest);

            var allSubthemesForTheme = _subthemeRepository.GetAllSubthemesForTheme(ModelsSetup.ThemeUnderTestName);

            allSubthemesForTheme.Should().HaveCount(ModelsSetup.ListOfSubthemesUnderTest.Length);
        }

        [TestMethod]
        public void GivenAddOrUpdateSubtheme_WhenNullSubtheme_ThenReturnsNull()
        {
            Subtheme subthemeUnderTest = null;

            var subtheme = _subthemeRepository.AddOrUpdateSubtheme(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void GivenAddOrUpdateSubtheme_WhenInvalidSubtheme_ThenReturnsNull(string subthemeName)
        {
            var subthemeUnderTest = new Subtheme { Name = subthemeName };

            var subtheme = _subthemeRepository.AddOrUpdateSubtheme(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateSubtheme_WhenNullTheme_ThenReturnsNull()
        {
            var subthemeUnderTest = new Subtheme { Name = ModelsSetup.NonExistentSubthemeName };

            var subtheme = _subthemeRepository.AddOrUpdateSubtheme(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void GivenAddOrUpdateSubtheme_WhenInvalidTheme_ThenReturnsNull(string themeName)
        {
            var subthemeUnderTest = new Subtheme { Name = ModelsSetup.NonExistentSubthemeName, Theme = new Theme { Name = themeName } };

            var subtheme = _subthemeRepository.AddOrUpdateSubtheme(subthemeUnderTest);

            subtheme.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateSubtheme_WhenNewValidSubtheme_ThenInsertsModel()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            _subthemeRepository.AddOrUpdateSubtheme(ModelsSetup.SubthemeUnderTest);

            var subtheme = _subthemeRepository.GetSubtheme(ModelsSetup.ThemeUnderTest.Name, ModelsSetup.SubthemeUnderTest.Name);

            subtheme.Should().BeEquivalentTo(ModelsSetup.SubthemeUnderTest);
        }

        [TestMethod]
        public void GivenAddOrUpdateSubtheme_WhenExistingValidSubtheme_ThenUpdatesModel()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            _subthemeRepository.AddOrUpdateSubtheme(ModelsSetup.SubthemeUnderTest);

            var subthemeUnderTest = _subthemeRepository.GetSubtheme(ModelsSetup.ThemeUnderTest.Name, ModelsSetup.SubthemeUnderTest.Name);

            subthemeUnderTest.SetCount = 66;
            subthemeUnderTest.YearTo = 2099;

            _subthemeRepository.AddOrUpdateSubtheme(subthemeUnderTest);

            var subtheme = _subthemeRepository.GetSubtheme(ModelsSetup.ThemeUnderTest.Name, ModelsSetup.SubthemeUnderTest.Name);

            subtheme.Should().BeEquivalentTo(subthemeUnderTest);
        }
    }
}
