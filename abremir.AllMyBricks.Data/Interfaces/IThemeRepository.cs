using System.Collections.Generic;
using abremir.AllMyBricks.Data.Models;

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
