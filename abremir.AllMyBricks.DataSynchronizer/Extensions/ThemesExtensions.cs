using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class ThemesExtensions
    {
        public static Theme ToTheme(this Themes source)
        {
            return new Theme
            {
                Name = source.Theme.Trim(),
                SetCount = (short)source.SetCount,
                SubthemeCount = (short)source.SubthemeCount,
                YearFrom = (short)source.YearFrom,
                YearTo = (short)source.YearTo
            };
        }
    }
}