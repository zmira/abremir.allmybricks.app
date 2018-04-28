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
    public class ThemeYearCountRepositoryTests : TestRepositoryBase
    {
        private static IThemeYearCountRepository _themeYearCountRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RCS1163 // Unused parameter.
        {
            _themeYearCountRepository = new ThemeYearCountRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void GivenGetThemeYearCount_WhenThemeNameNotValid_ThenReturnsNull(string themeName)
        {
            var themeYearCount = _themeYearCountRepository.GetThemeYearCount(themeName, 2000);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenGetThemeYearCount_WhenYearIsLessThanMinimumConstant_ThenReturnsNull()
        {
            var themeYearCount = _themeYearCountRepository.GetThemeYearCount(ModelsSetup.ThemeUnderTestName, Constants.MinimumSetYear - 1);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenGetThemeYearCount_WhenThemeDoesNotExists_ThenReturnsNull()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var themeYearCount = _themeYearCountRepository
                .GetThemeYearCount(ModelsSetup.NonExistentThemeName, ModelsSetup.ThemeYearCountUnderTest.Year);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenGetThemeYearCount_WhenYearDoesNotExists_ThenReturnsNull()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var themeYearCount = _themeYearCountRepository
                .GetThemeYearCount(ModelsSetup.ThemeUnderTestName, (ushort)(ModelsSetup.ThemeYearCountUnderTest.Year - 1));

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenGetThemeYearCount_WhenThemeAndYearExist_ThenReturnsModel()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var themeYearCount = _themeYearCountRepository
                .GetThemeYearCount(ModelsSetup.ThemeUnderTestName, ModelsSetup.ThemeYearCountUnderTest.Year);

            themeYearCount.Should().BeEquivalentTo(ModelsSetup.ThemeYearCountUnderTest);
        }

        [TestMethod]
        public void GivenGetAllThemeYearCount_WhenNoThemeYearCount_ThenReturnsEmpty()
        {
            var allThemeYearCount = _themeYearCountRepository.GetAllThemeYearCount();

            allThemeYearCount.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllThemeYearCount_WhenHasThemeYearCount_ThenReturnsModels()
        {
            InsertData(ModelsSetup.ListOfThemesUnderTest);
            InsertData(ModelsSetup.ListOfThemeYearCountUnderTest);

            var allThemeYearCount = _themeYearCountRepository.GetAllThemeYearCount();

            allThemeYearCount.Should().BeEquivalentTo(ModelsSetup.ListOfThemeYearCountUnderTest);
        }

        [TestMethod]
        public void GivenGetAllThemeYearCountForYear_WhenNoThemeYearCountForYear_ThenReturnsEmpty()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var allThemeYearCountForYear = _themeYearCountRepository
                .GetAllThemeYearCountForYear((ushort)(ModelsSetup.ThemeYearCountUnderTest.Year - 1));

            allThemeYearCountForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllThemeYearCountForYear_WhenYearIsLessThanMinimumConstant_ThenReturnsEmpty()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var allThemeYearCountForYear = _themeYearCountRepository.GetAllThemeYearCountForYear(Constants.MinimumSetYear - 1);

            allThemeYearCountForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllThemeYearCountForYear_WhenHasThemeYearCountForYear_ThenReturnsModels()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var allThemeYearCountForYear = _themeYearCountRepository.GetAllThemeYearCountForYear(ModelsSetup.ThemeYearCountUnderTest.Year);

            allThemeYearCountForYear.Should().HaveCount(1);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void GivenGetAllThemeYearCountForTheme_WhenThemeNameNotValid_ThenReturnsEmpty(string themeName)
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var allThemeYearCountForYear = _themeYearCountRepository.GetAllThemeYearCountForTheme(themeName);

            allThemeYearCountForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllThemeYearCountForTheme_WhenNoThemeYearCountForTheme_ThenReturnsEmpty()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var allThemeYearCountForYear = _themeYearCountRepository.GetAllThemeYearCountForTheme(ModelsSetup.NonExistentThemeName);

            allThemeYearCountForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void GivenGetAllThemeYearCountForTheme_WhenHasThemeYearCountForTheme_ThenReturnsModels()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            InsertData(ModelsSetup.ThemeYearCountUnderTest);

            var allThemeYearCountForYear = _themeYearCountRepository.GetAllThemeYearCountForTheme(ModelsSetup.ThemeUnderTestName);

            allThemeYearCountForYear.Should().HaveCount(1);
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenNullThemeYearCount_ThenReturnsNull()
        {
            ThemeYearCount themeYearCountUnderTest = null;

            var themeYearCount = _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenNullTheme_ThenReturnsNull()
        {
            var themeYearCountUnderTest = new ThemeYearCount
            {
                Key = new ThemeYear
                {

                }
            };

            var themeYearCount = _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            themeYearCount.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void GivenAddOrUpdateThemeYearCount_WhenThemeNameNotValid_ThenReturnsNull(string themeName)
        {
            var themeYearCountUnderTest = new ThemeYearCount
            {
                Key = new ThemeYear
                {
                    Theme = new Theme
                    {
                        Name = themeName
                    },
                    Year = Constants.MinimumSetYear + 1
                }
            };

            var themeYearCount = _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenYearIsLessThanMinimumConstant_ThenReturnsNull()
        {
            var themeYearCountUnderTest = new ThemeYearCount
            {
                Key = new ThemeYear
                {
                    Theme = new Theme
                    {
                        Name = ModelsSetup.NonExistentThemeName
                    },
                    Year = Constants.MinimumSetYear - 1
                }
            };

            var themeYearCount = _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenYearIsLessThanThemeYearFrom_ThenReturnsNull()
        {
            var themeYearCountUnderTest = new ThemeYearCount
            {
                Key = new ThemeYear
                {
                    Theme = new Theme
                    {
                        Name = ModelsSetup.NonExistentThemeName,
                        YearFrom = 2000
                    },
                    Year = 1999
                }
            };

            var themeYearCount = _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenYearIsMoreThanThemeYearTo_ThenReturnsNull()
        {
            var themeYearCountUnderTest = new ThemeYearCount
            {
                Key = new ThemeYear
                {
                    Theme = new Theme
                    {
                        Name = ModelsSetup.NonExistentThemeName,
                        YearFrom = 2000,
                        YearTo = 2015
                    },
                    Year = 2016
                }
            };

            var themeYearCount = _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenThemeYearFromIsLessThanMinimumConstant_ThenReturnsNull()
        {
            var themeYearCountUnderTest = new ThemeYearCount
            {
                Key = new ThemeYear
                {
                    Theme = new Theme
                    {
                        Name = ModelsSetup.NonExistentThemeName,
                        YearFrom = Constants.MinimumSetYear - 1
                    },
                    Year = Constants.MinimumSetYear + 1
                }
            };

            var themeYearCount = _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            themeYearCount.Should().BeNull();
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenNewValidThemeYearCount_ThenInsertsModel()
        {
            InsertData(ModelsSetup.ThemeUnderTest);

            var themeYearCountUnderTest = new ThemeYearCount
            {
                Key = new ThemeYear
                {
                    Theme = ModelsSetup.ThemeUnderTest,
                    Year = (ushort)(ModelsSetup.ThemeUnderTest.YearFrom + 1)
                },
                Count = 10
            };

            _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            var themeYearCount = _themeYearCountRepository.GetThemeYearCount(themeYearCountUnderTest.Theme.Name, themeYearCountUnderTest.Year);

            themeYearCount.Should().BeEquivalentTo(themeYearCountUnderTest);
        }

        [TestMethod]
        public void GivenAddOrUpdateThemeYearCount_WhenExistingValidThemeYearCount_ThenUpdatesModel()
        {
            InsertData(ModelsSetup.ThemeUnderTest);
            _themeYearCountRepository.AddOrUpdateThemeYearCount(ModelsSetup.ThemeYearCountUnderTest);

            var themeYearCountUnderTest = _themeYearCountRepository
                .GetThemeYearCount(ModelsSetup.ThemeUnderTestName, ModelsSetup.ThemeUnderTest.YearFrom);

            themeYearCountUnderTest.Count = 777;

            _themeYearCountRepository.AddOrUpdateThemeYearCount(themeYearCountUnderTest);

            var themeYearCount = _themeYearCountRepository.GetThemeYearCount(ModelsSetup.ThemeUnderTestName, ModelsSetup.ThemeUnderTest.YearFrom);

            themeYearCount.Should().BeEquivalentTo(themeYearCountUnderTest);
        }
    }
}