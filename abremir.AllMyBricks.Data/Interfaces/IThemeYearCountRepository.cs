using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IThemeYearCountRepository
    {
        ThemeYearCount AddOrUpdateThemeYearCount(ThemeYearCount themeYearCount);
        ThemeYearCount GetThemeYearCount(string themeName, ushort year);
        IEnumerable<ThemeYearCount> GetAllThemeYearCount();
        IEnumerable<ThemeYearCount> GetAllThemeYearCountForTheme(string themeName);
        IEnumerable<ThemeYearCount> GetAllThemeYearCountForYear(ushort year);
    }
}