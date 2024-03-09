using System;
using System.Linq;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using abremir.AllMyBricks.Data.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class SetRepositoryTests : DataTestsBase
    {
        private static ISetRepository _setRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _setRepository = new SetRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public void Get_InvalidSetId_ReturnsNull()
        {
            var set = _setRepository.Get(0);

            Check.That(set).IsNull();
        }

        [TestMethod]
        public void Get_SetDoesNotExist_ReturnsNull()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            InsertData(setUnderTest);

            var set = _setRepository.Get(setUnderTest.SetId + 1);

            Check.That(set).IsNull();
        }

        [TestMethod]
        public void Get_SetExists_ReturnsModel()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            InsertData(setUnderTest);

            var set = _setRepository.Get(setUnderTest.SetId);

            Check.That(set.SetId).IsEqualTo(setUnderTest.SetId);
        }

        [TestMethod]
        public void All_NoSets_ReturnsEmpty()
        {
            var allSets = _setRepository.All();

            Check.That(allSets).IsEmpty();
        }

        [TestMethod]
        public void All_HasSets_ReturnsModels()
        {
            var listOfSetsUnderTest = ModelsSetup.ListOfSetsUnderTest;

            InsertData(listOfSetsUnderTest);

            var allSets = _setRepository.All();

            Check.That(allSets.Select(set => set.SetId)).IsEquivalentTo(listOfSetsUnderTest.Select(set => set.SetId));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForTheme_InvalidThemeName_ReturnsEmpty(string themeName)
        {
            var allSetsForTheme = _setRepository.AllForTheme(themeName);

            Check.That(allSetsForTheme).IsEmpty();
        }

        [TestMethod]
        public void AllForTheme_SetForThemeDoesNotExist_ReturnsEmpty()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Theme = InsertData(ModelsSetup.GetThemeUnderTest(Guid.NewGuid().ToString()));

            InsertData(set);

            var allSetsForTheme = _setRepository.AllForTheme(ModelsSetup.NonExistentThemeName);

            Check.That(allSetsForTheme).IsEmpty();
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

            Check.That(allSetsForTheme).CountIs(1);
            Check.That(allSetsForTheme.First().SetId).IsEqualTo(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow(null, ModelsSetup.StringEmpty)]
        [DataRow(ModelsSetup.StringEmpty, null)]
        [DataRow(ModelsSetup.StringEmpty, ModelsSetup.StringEmpty)]
        public void AllForSubtheme_InvalidSubthemeName_ReturnsEmpty(string themeName, string subthemeName)
        {
            var allSetsForSubtheme = _setRepository.AllForSubtheme(themeName, subthemeName);

            Check.That(allSetsForSubtheme).IsEmpty();
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

            Check.That(allSetsForSubtheme).IsEmpty();
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

            Check.That(allSetsForSubtheme).CountIs(1);
            Check.That(allSetsForSubtheme.First().SetId).IsEqualTo(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForThemeGroup_InvalidThemeGroupName_ReturnsEmpty(string themeGroupName)
        {
            var allSetsForThemeGroup = _setRepository.AllForThemeGroup(themeGroupName);

            Check.That(allSetsForThemeGroup).IsEmpty();
        }

        [TestMethod]
        public void AllForThemeGroup_ThemeGroupDoesNotExist_ReturnsEmpty()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].ThemeGroup = InsertData(ModelsSetup.ThemeGroupReferenceData);

            InsertData(listOfSets);

            var allSetsForThemeGroup = _setRepository.AllForThemeGroup($"{ModelsSetup.ThemeGroupReferenceDataValue}_NON-EXISTENT");

            Check.That(allSetsForThemeGroup).IsEmpty();
        }

        [TestMethod]
        public void AllForThemeGroup_ThemeGroupExists_ReturnsModels()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].ThemeGroup = InsertData(ModelsSetup.ThemeGroupReferenceData);

            InsertData(listOfSets);

            var allSetsForThemeGroup = _setRepository.AllForThemeGroup(ModelsSetup.ThemeGroupReferenceDataValue);

            Check.That(allSetsForThemeGroup).CountIs(1);
            Check.That(allSetsForThemeGroup.First().SetId).IsEqualTo(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForCategory_InvalidCategoryName_ReturnsEmpty(string categoryName)
        {
            var allSetsForCategory = _setRepository.AllForCategory(categoryName);

            Check.That(allSetsForCategory).IsEmpty();
        }

        [TestMethod]
        public void AllForCategory_CategoryDoesNotExist_ReturnsEmpty()
        {
            var category = InsertData(ModelsSetup.CategoryReferenceData);

            var setUnderTest = ModelsSetup.GetSetUnderTest();
            setUnderTest.Category = category;

            InsertData(setUnderTest);

            var allSetsForCategory = _setRepository.AllForCategory($"{ModelsSetup.CategoryReferenceDataValue}_NON-EXISTENT");

            Check.That(allSetsForCategory).IsEmpty();
        }

        [TestMethod]
        public void AllForCategory_CategoryExists_ReturnsModels()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Category = InsertData(ModelsSetup.CategoryReferenceData);

            InsertData(listOfSets);

            var allSetsForCategory = _setRepository.AllForCategory(ModelsSetup.CategoryReferenceDataValue);

            Check.That(allSetsForCategory).CountIs(1);
            Check.That(allSetsForCategory.First().SetId).IsEqualTo(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AllForTag_InvalidTagName_ReturnsEmpty(string tagName)
        {
            var allSetsForTag = _setRepository.AllForTag(tagName);

            Check.That(allSetsForTag).IsEmpty();
        }

        [TestMethod]
        public void AllForTag_TagDoesNotExist_ReturnsEmpty()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Tags.Add(InsertData(ModelsSetup.TagReferenceData));

            InsertData(listOfSets);

            var allSetsForTag = _setRepository.AllForTag($"{ModelsSetup.TagReferenceDataValue}_NON-EXISTENT");

            Check.That(allSetsForTag).IsEmpty();
        }

        [TestMethod]
        public void AllForTag_TagExists_ReturnsModels()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Tags.Add(InsertData(ModelsSetup.TagReferenceData));

            InsertData(listOfSets);

            var allSetsForTag = _setRepository.AllForTag(ModelsSetup.TagReferenceDataValue);

            Check.That(allSetsForTag).CountIs(1);
            Check.That(allSetsForTag.First().SetId).IsEqualTo(listOfSets[0].SetId);
        }

        [TestMethod]
        public void AllForYear_InvalidYear_ReturnsEmpty()
        {
            var allSetsForYear = _setRepository.AllForYear(Constants.MinimumSetYear - 1);

            Check.That(allSetsForYear).IsEmpty();
        }

        [TestMethod]
        public void AllForYear_YearDoesNotExist_ReturnsEmpty()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Year = (short)Constants.MinimumSetYear;
            listOfSets[1].Year = Constants.MinimumSetYear + 1;

            InsertData(listOfSets);

            var allSetsForYear = _setRepository.AllForYear((short)Constants.MinimumSetYear + 2);

            Check.That(allSetsForYear).IsEmpty();
        }

        [TestMethod]
        public void AllForYear_YearExists_ReturnsModel()
        {
            var listOfSets = ModelsSetup.ListOfSetsUnderTest;
            listOfSets[0].Year = (short)Constants.MinimumSetYear;
            listOfSets[1].Year = Constants.MinimumSetYear + 1;

            InsertData(listOfSets);

            var allSetsForYear = _setRepository.AllForYear((short)Constants.MinimumSetYear);

            Check.That(allSetsForYear).CountIs(1);
            Check.That(allSetsForYear.First().SetId).IsEqualTo(listOfSets[0].SetId);
        }

        [DataTestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        public void AllForPriceRange_InvalidPrice_ReturnsEmpty(float minPrice, float maxPrice)
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegion.CA,
                Value = 1
            });

            InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegion.CA, minPrice, maxPrice);

            Check.That(allSetsForPriceRange).IsEmpty();
        }

        [TestMethod]
        public void AllForPriceRange_RegionDoesNotExists_ReturnsEmpty()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegion.CA,
                Value = 1
            });

            InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegion.DE, 0, 10);

            Check.That(allSetsForPriceRange).IsEmpty();
        }

        [TestMethod]
        public void AllForPriceRange_PriceDoesNotExist_ReturnsEmpty()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegion.CA,
                Value = 10
            });

            InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegion.CA, 0, 5);

            Check.That(allSetsForPriceRange).IsEmpty();
        }

        [TestMethod]
        public void AllForPriceRange_PriceExists_ReturnsModel()
        {
            var set = ModelsSetup.GetSetUnderTest();
            set.Prices.Add(new Price
            {
                Region = PriceRegion.CA,
                Value = 1
            });

            InsertData(set);

            var allSetsForPriceRange = _setRepository.AllForPriceRange(PriceRegion.CA, 0, 5);

            Check.That(allSetsForPriceRange).CountIs(1);
            Check.That(allSetsForPriceRange.First().SetId).IsEqualTo(set.SetId);
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
        public void SearchBy_InvalidSearchTerm_ReturnsEmpty(string searchTerm)
        {
            var searchResult = _setRepository.SearchBy(searchTerm);

            Check.That(searchResult).IsEmpty();
        }

        [TestMethod]
        public void SearchBy_SearchTermDoesNotExist_ReturnsEmpty()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();
            setUnderTest.Name = "SETUNDERTEST";

            InsertData(setUnderTest);

            var searchResult = _setRepository.SearchBy($"{setUnderTest.Name}_NONEXISTENT");

            Check.That(searchResult).IsEmpty();
        }

        [DataTestMethod]
        [DataRow("name")]
        [DataRow("number")]
        [DataRow("ean")]
        [DataRow("upc")]
        [DataRow("theme")]
        [DataRow("subtheme")]
        [DataRow("themegroup")]
        [DataRow("packagingtype")]
        [DataRow("category")]
        [DataRow("tag")]
        public void SearchBy_SearchTermExists_ReturnsSearchResult(string searchTerm)
        {
            var setUnderTest = SetupSetForSearch(0);

            var searchResult = _setRepository.SearchBy(searchTerm);

            Check.That(searchResult).CountIs(1);
            Check.That(searchResult.First().SetId).IsEqualTo(setUnderTest.SetId);
        }

        [TestMethod]
        public void SearchBy_MultipleSearchTerms_ReturnsSearchResult()
        {
            SetupSetForSearch(0);
            SetupSetForSearch(1);

            var searchResult = _setRepository.SearchBy("tag1 subtheme0");
            Check.That(searchResult).CountIs(2);
        }

        [TestMethod]
        public void AddOrUpdate_NullSet_ReturnsNull()
        {
            var set = _setRepository.AddOrUpdate(null);

            Check.That(set).IsNull();
        }

        [TestMethod]
        public void AddOrUpdate_InvalidSet_ReturnsNull()
        {
            var set = _setRepository.AddOrUpdate(new Set { SetId = 0 });

            Check.That(set).IsNull();
        }

        [TestMethod]
        public void AddOrUpdate_NewValidSet_InsertsModel()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            _setRepository.AddOrUpdate(setUnderTest);

            var set = _setRepository.Get(setUnderTest.SetId);

            Check.That(set).IsNotNull();
            Check.That(set.SetId).IsEqualTo(setUnderTest.SetId);
        }

        [TestMethod]
        public void AddOrUpdate_ExistingValidSet_UpdatesModel()
        {
            var setUnderTest = ModelsSetup.GetSetUnderTest();

            _setRepository.AddOrUpdate(setUnderTest);

            var setFromDb = _setRepository.Get(setUnderTest.SetId);

            setFromDb.Name = "NEW NAME";

            _setRepository.AddOrUpdate(setFromDb);

            var set = _setRepository.Get(setUnderTest.SetId);

            Check.That(set).HasFieldsWithSameValues(setFromDb);
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
            set.PackagingType = InsertData(new PackagingType { Value = $"SET PACKAGINGTYPE{suffix}" });
            set.Category = InsertData(new Category { Value = $"SET CATEGORY{suffix}" });
            set.Tags.Add(InsertData(new Tag { Value = $"SET TAG{suffix}" }));

            return InsertData(set);
        }
    }
}
