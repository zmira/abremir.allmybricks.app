using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Enumerations;
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
    public class SetRepositoryTests : TestRepositoryBase
    {
        private static ISetRepository _setRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RCS1163 // Unused parameter.
        {
            _setRepository = new SetRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public void Get_SetIdNotValid_ReturnsNull()
        {
            var set = _setRepository.Get(0);

            set.Should().BeNull();
        }

        [TestMethod]
        public void Get_SetDoesNotExist_ReturnsNull()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            InsertData(setUnderTest);

            var set = _setRepository.Get(setUnderTest.SetId + 1);

            set.Should().BeNull();
        }

        [TestMethod]
        public void Get_SetExists_ReturnsModel()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            InsertData(setUnderTest);

            var set = _setRepository.Get(setUnderTest.SetId);

            set.SetId.Should().Be(setUnderTest.SetId);
        }

        [TestMethod]
        public void All_NoSets_ReturnsEmpty()
        {
            var allSets = _setRepository.All();

            allSets.Should().BeEmpty();
        }

        [TestMethod]
        public void All_HasSets_ReturnsModels()
        {
            var listOfSetsUnderTest = ModelsSetup.ListOfSetsUnderTest;

            InsertData(listOfSetsUnderTest);

            var allSets = _setRepository.All();

            allSets.Select(set => set.SetId).Should().BeEquivalentTo(listOfSetsUnderTest.Select(set => set.SetId));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForTheme_ThemeNameNotValid_ReturnsEmpty(string themeName)
        {
            var allSetsForTheme = _setRepository.AllForTheme(themeName);

            allSetsForTheme.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForTheme_SetForThemeDoesNotExist_ReturnsEmpty()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Theme = InsertData(ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString()));

            InsertData(set);

            var allSetsForTheme = _setRepository.AllForTheme(ModelsSetup.NonExistentThemeName);

            allSetsForTheme.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForTheme_SetForThemeExists_ReturnsModels()
        {
            var listOfThemes = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Theme = listOfThemes[0];
            listOfSets[1].Theme = listOfThemes[1];

            InsertData(listOfSets);

            var allSetsForTheme = _setRepository.AllForTheme(listOfThemes[0].Name);

            allSetsForTheme.Should().HaveCount(1);
            allSetsForTheme.First().SetId.Should().Be(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(null, ModelsSetup.StringEmpty)]
        [DataRow(ModelsSetup.StringEmpty, null)]
        [DataRow(ModelsSetup.StringEmpty, ModelsSetup.StringEmpty)]
        public void AllForSubtheme_SubthemeNameNotValid_ReturnsEmpty(string themeName, string subthemeName)
        {
            var allSetsForSubtheme = _setRepository.AllForSubtheme(themeName, subthemeName);

            allSetsForSubtheme.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForSubtheme_SetForSubthemeDoesNotExist_ReturnsEmpty()
        {
            var listOfThemes = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemes = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemes[0].Theme = listOfThemes[0];
            listOfSubthemes[1].Theme = listOfThemes[1];

            var listOfSubthemesUnderTest = InsertData(listOfSubthemes);

            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Theme = listOfThemes[0];
            listOfSets[0].Subtheme = listOfSubthemesUnderTest[0];
            listOfSets[1].Theme = listOfThemes[1];
            listOfSets[1].Subtheme = listOfSubthemesUnderTest[1];

            InsertData(listOfSets);

            var allSetsForSubtheme = _setRepository.AllForSubtheme(listOfSets[0].Theme.Name, ModelsSetup.NonExistentSubthemeName);

            allSetsForSubtheme.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForSubtheme_SetForSubthemeExists_ReturnsModels()
        {
            var listOfThemes = InsertData(ModelsSetup.ListOfThemesUnderTest);

            var listOfSubthemes = ModelsSetup.ListOfSubthemesUnderTest;
            listOfSubthemes[0].Theme = listOfThemes[0];
            listOfSubthemes[1].Theme = listOfThemes[1];

            var listOfSubthemesUnderTest = InsertData(listOfSubthemes);

            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Theme = listOfThemes[0];
            listOfSets[0].Subtheme = listOfSubthemesUnderTest[0];
            listOfSets[1].Theme = listOfThemes[1];
            listOfSets[1].Subtheme = listOfSubthemesUnderTest[1];

            InsertData(listOfSets);

            var allSetsForSubtheme = _setRepository.AllForSubtheme(listOfSets[0].Theme.Name, listOfSets[0].Subtheme.Name);

            allSetsForSubtheme.Should().HaveCount(1);
            allSetsForSubtheme.First().SetId.Should().Be(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForThemeGroup_ThemeGroupNameNotValid_ReturnsEmpty(string themeGroupName)
        {
            var allSetsForThemeGroup = _setRepository.AllForThemeGroup(themeGroupName);

            allSetsForThemeGroup.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForThemeGroup_ThemeGroupDoesNotExist_ReturnsEmpty()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].ThemeGroup = InsertData(ModelsSetup.ThemeGroupReferenceData);

            InsertData(listOfSets);

            var allSetsForThemeGroup = _setRepository.AllForThemeGroup($"{ModelsSetup.ThemeGroupReferenceDataValue}_NON-EXISTENT");

            allSetsForThemeGroup.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForThemeGroup_ThemeGroupExists_ReturnsModels()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].ThemeGroup = InsertData(ModelsSetup.ThemeGroupReferenceData);

            InsertData(listOfSets);

            var allSetsForThemeGroup = _setRepository.AllForThemeGroup(ModelsSetup.ThemeGroupReferenceDataValue);

            allSetsForThemeGroup.Should().HaveCount(1);
            allSetsForThemeGroup.First().SetId.Should().Be(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForCategory_CategoryNameNotValid_ReturnsEmpty(string categoryName)
        {
            var allSetsForCategory = _setRepository.AllForCategory(categoryName);

            allSetsForCategory.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForCategory_CategoryDoesNotExist_ReturnsEmpty()
        {
            var category = InsertData(ModelsSetup.CategoryReferenceData);

            var setUnderTest = ModelsSetup.GetSetUnderTest();
            setUnderTest.Category = category;

            InsertData(setUnderTest);

            var allSetsForCategory = _setRepository.AllForCategory($"{ModelsSetup.CategoryReferenceDataValue}_NON-EXISTENT");

            allSetsForCategory.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForCategory_CategoryExists_ReturnsModels()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Category = InsertData(ModelsSetup.CategoryReferenceData);

            InsertData(listOfSets);

            var allSetsForCategory = _setRepository.AllForCategory(ModelsSetup.CategoryReferenceDataValue);

            allSetsForCategory.Should().HaveCount(1);
            allSetsForCategory.First().SetId.Should().Be(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForTag_TagNameNotValid_ReturnsEmpty(string tagName)
        {
            var allSetsForTag = _setRepository.AllForTag(tagName);

            allSetsForTag.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForTag_TagDoesNotExist_ReturnsEmpty()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Tags.Add(InsertData(ModelsSetup.TagReferenceData));

            InsertData(listOfSets);

            var allSetsForTag = _setRepository.AllForTag($"{ModelsSetup.TagReferenceDataValue}_NON-EXISTENT");

            allSetsForTag.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForTag_TagExists_ReturnsModels()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Tags.Add(InsertData(ModelsSetup.TagReferenceData));

            InsertData(listOfSets);

            var allSetsForTag = _setRepository.AllForTag(ModelsSetup.TagReferenceDataValue);

            allSetsForTag.Should().HaveCount(1);
            allSetsForTag.First().SetId.Should().Be(listOfSets[0].SetId);
        }

        [TestMethod]
        public void AllForYear_YearNotValid_ReturnsEmpty()
        {
            var allSetsForYear = _setRepository.AllForYear(Constants.MinimumSetYear - 1);

            allSetsForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForYear_YearDoesNotExist_ReturnsEmpty()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Year = (short?)Constants.MinimumSetYear;
            listOfSets[1].Year = Constants.MinimumSetYear + 1;

            InsertData(listOfSets);

            var allSetsForYear = _setRepository.AllForYear((short)Constants.MinimumSetYear + 2);

            allSetsForYear.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForYear_YearExists_ReturnsModel() {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Year = (short?)Constants.MinimumSetYear;
            listOfSets[1].Year = Constants.MinimumSetYear + 1;

            InsertData(listOfSets);

            var allSetsForYear = _setRepository.AllForYear((short)Constants.MinimumSetYear);

            allSetsForYear.Should().HaveCount(1);
            allSetsForYear.First().SetId.Should().Be(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        public void AllForPriceRange_PriceNotValid_ReturnsEmpty(float minPrice, float maxPrice)
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegionEnum.CA,
                Value = 1
            });

            var setUnderTest = InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegionEnum.CA, minPrice, maxPrice);

            allSetsForPriceRange.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForPriceRange_RegionDoesNotExists_ReturnsEmpty()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegionEnum.CA,
                Value = 1
            });

            var setUnderTest = InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegionEnum.EU, 0, 10);

            allSetsForPriceRange.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForPriceRange_PriceDoesNotExist_ReturnsEmpty()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegionEnum.CA,
                Value = 10
            });

            var setUnderTest = InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegionEnum.CA, 0, 5);

            allSetsForPriceRange.Should().BeEmpty();
        }

        [TestMethod]
        public void AllForPriceRange_PriceExists_ReturnsModel()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegionEnum.CA,
                Value = 1
            });

            var setUnderTest = InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegionEnum.CA, 0, 5);

            allSetsForPriceRange.Should().HaveCount(1);
            allSetsForPriceRange.First().SetId.Should().Be(set.SetId);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("-")]
        [DataRow("a-a")]
        [DataRow("ab-ab")]
        [DataRow("a a")]
        [DataRow("ab ab")]
        public void SearchBy_SearchTermNotValid_ReturnsEmpty(string searchTerm)
        {
            var searchResult = _setRepository.SearchBy(searchTerm);

            searchResult.Should().BeEmpty();
        }

        [TestMethod]
        public void SearchBy_SearchTermDoesNotExist_ReturnsEmpty()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();
            setUnderTest.Name = "SETUNDERTEST";

            InsertData(setUnderTest);

            var searchResult = _setRepository.SearchBy($"{setUnderTest.Name}_NONEXISTANT");

            searchResult.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("name")]
        [DataRow("number")]
        [DataRow("ean")]
        [DataRow("upc")]
        [DataRow("description")]
        [DataRow("theme")]
        [DataRow("subtheme")]
        [DataRow("themegroup")]
        [DataRow("category")]
        [DataRow("tag")]
        public void SearchBy_SearchTermExists_ReturnsSearchResult(string searchTerm)
        {
            var setUnderTest = SetupSetForSearch(0);

            var searchResult = _setRepository.SearchBy(searchTerm);

            searchResult.Should().HaveCount(1);
            searchResult.First().SetId.Should().Be(setUnderTest.SetId);
        }

        [TestMethod]
        public void SearchBy_MultipleSearchTerms_ReturnsSearchResult()
        {
            var setUnderTest0 = SetupSetForSearch(0);
            var setUnderTest1 = SetupSetForSearch(1);

            var searchResult = _setRepository.SearchBy("subtheme0 tag1");
            searchResult.Should().HaveCount(2);
        }

        private Set SetupSetForSearch(int suffix)
        {
            var theme = InsertData(ModelsSetup.GetThemeUnderTest($"SET THEME{suffix}"));

            var subtheme = ModelsSetup.GetSubthemeUnderTest($"SET SUBTHEME{suffix}");
            subtheme.Theme = theme;

            subtheme = InsertData(subtheme);

            var set = ModelsSetup.GetSetForSearch(suffix);
            set.SetId = suffix;
            set.Theme = theme;
            set.Subtheme = subtheme;
            set.ThemeGroup = InsertData(new ThemeGroup { Value = $"SET THEMEGROUP{suffix}" });
            set.Category = InsertData(new Category { Value = $"SET CATEGORY{suffix}" });
            set.Tags.Add(InsertData(new Tag { Value = $"SET TAG{suffix}" }));

            return InsertData(set);
        }

        [TestMethod]
        public void AddOrUpdate_NullSet_ReturnsNull()
        {
            var set = _setRepository.AddOrUpdate(null);

            set.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_InvalidSet_ReturnsNull()
        {
            var set = _setRepository.AddOrUpdate(new Set { SetId = 0 });

            set.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdate_NewValidSet_InsertsModel()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            _setRepository.AddOrUpdate(setUnderTest);

            var set = _setRepository.Get(setUnderTest.SetId);

            set.Should().NotBeNull();
            set.SetId.Should().Be(setUnderTest.SetId);
        }

        [TestMethod]
        public void AddOrUpdate_ExistingValidSet_UpdatesModel()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            _setRepository.AddOrUpdate(setUnderTest);

            var result = _setRepository.Get(setUnderTest.SetId);

            result.Name = "NEW NAME";

            _setRepository.AddOrUpdate(result);

            var set = _setRepository.Get(setUnderTest.SetId);

            set.Should().BeEquivalentTo(result);
        }
    }
}