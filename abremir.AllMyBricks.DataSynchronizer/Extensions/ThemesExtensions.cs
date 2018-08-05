using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class ThemesExtensions
    {
        public static Theme ToTheme(this Themes source)
        {
            return new Theme
            {
                Name = source.Theme,
                SetCount = (short)source.SetCount,
                SubthemeCount = (short)source.SubthemeCount,
                YearFrom = (short)source.YearFrom,
                YearTo = (short)source.YearTo
            };
        }

        public static IEnumerable<Theme> ToTheme(this IEnumerable<Themes> source)
        {
            foreach (var item in source)
            {
                yield return item.ToTheme();
            }
        }
    }
}