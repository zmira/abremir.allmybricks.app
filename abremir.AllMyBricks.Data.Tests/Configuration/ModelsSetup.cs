using abremir.AllMyBricks.Data.Models;
using System;

namespace abremir.AllMyBricks.Data.Tests.Configuration
{
    public static class ModelsSetup
    {
        public const string StringEmpty = "";
        public const string ThemeUnderTestName = "test theme";
        public const string SubthemeUnderTestName = "test subtheme";
        public const string NonExistentThemeName = "unknown theme";
        public const string NonExistentSubthemeName = "unknown subtheme";
        public const short FirstThemeYearFrom = 1990;
        public const short FirstThemeYearTo = 2019;
        public const short SecondThemeYearTo = 2020;
        public const short FirstSubthemeYearFrom = FirstThemeYearFrom + 10;
        public const short SecondSubthemeYearFrom = FirstThemeYearFrom + 11;
        public const string CategoryReferenceDataValue = "New Category";
        public const string PackagingTypeReferenceDataValue = "New Packaging Type";
        public const string TagReferenceDataValue = "New Tag";
        public const string ThemeGroupReferenceDataValue = "New Theme Group";

        public static Theme GetThemeUnderTest(string themeName)
        {
            var theme = new Theme
            {
                Name = themeName,
                SetCount = 9,
                SubthemeCount = 99,
                YearFrom = FirstThemeYearFrom,
                YearTo = FirstThemeYearTo
            };

            theme.SetCountPerYear.Add(new YearSetCount
            {
                Year = FirstThemeYearTo
            });

            return theme;
        }

        public static Theme GetSecondThemeUnderTest(string themeName)
        {
            var theme = new Theme
            {
                Name = themeName,
                SetCount = 10,
                SubthemeCount = 100,
                YearFrom = 1991,
                YearTo = SecondThemeYearTo
            };

            theme.SetCountPerYear.Add(new YearSetCount
            {
                Year = FirstThemeYearTo
            });

            theme.SetCountPerYear.Add(new YearSetCount
            {
                Year = SecondThemeYearTo
            });

            return theme;
        }

        public static Theme[] ListOfThemesUnderTest => new[] { GetThemeUnderTest(Guid.NewGuid().ToString()), GetSecondThemeUnderTest(Guid.NewGuid().ToString()) };

        public static Subtheme GetSubthemeUnderTest(string subthemeName)
        {
            var theme = GetThemeUnderTest(Guid.NewGuid().ToString());

            return new Subtheme
            {
                Name = subthemeName,
                SetCount = 8,
                Theme = theme,
                YearFrom = FirstSubthemeYearFrom,
                YearTo = theme.YearTo
            };
        }

        public static Subtheme GetSecondSubthemeUnderTest(string subthemeName)
        {
            var theme = GetThemeUnderTest(Guid.NewGuid().ToString());

            return new Subtheme
            {
                Name = subthemeName,
                SetCount = 7,
                Theme = theme,
                YearFrom = SecondSubthemeYearFrom,
                YearTo = theme.YearTo
            };
        }

        public static Subtheme[] ListOfSubthemesUnderTest => new[] { GetSubthemeUnderTest(Guid.NewGuid().ToString()), GetSecondSubthemeUnderTest(Guid.NewGuid().ToString()) };

        public static Category CategoryReferenceData => new Category
        {
            Value = CategoryReferenceDataValue
        };

        public static ThemeGroup ThemeGroupReferenceData => new ThemeGroup
        {
            Value = ThemeGroupReferenceDataValue
        };

        public static Tag TagReferenceData => new Tag
        {
            Value = TagReferenceDataValue
        };

        public static PackagingType PackagingTypeReferenceData => new PackagingType
        {
            Value = PackagingTypeReferenceDataValue
        };
    }
}
