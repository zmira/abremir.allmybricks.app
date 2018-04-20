using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IThemeRepository
    {
        Theme AddOrUpdateTheme(Theme theme);
        Theme GetTheme(string themeName);
        IEnumerable<Theme> GetAllThemes();
        IEnumerable<Theme> GetAllThemesForYear(ushort year);
    }
}