using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface ISubthemeRepository
    {
        Subtheme AddOrUpdate(Subtheme subtheme);
        Subtheme Get(string themeName, string subthemeName);
        IEnumerable<Subtheme> All();
        IEnumerable<Subtheme> AllForYear(short year);
        IEnumerable<Subtheme> AllForTheme(string themeName);
    }
}
