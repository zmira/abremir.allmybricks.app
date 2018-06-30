using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IThemeRepository
    {
        Theme AddOrUpdate(Theme theme);
        Theme Get(string themeName);
        IEnumerable<Theme> All();
        IEnumerable<Theme> AllForYear(short year);
    }
}