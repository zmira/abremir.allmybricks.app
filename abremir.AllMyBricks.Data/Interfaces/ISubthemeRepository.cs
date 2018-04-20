using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface ISubthemeRepository
    {
        Subtheme AddOrUpdateSubtheme(Subtheme subtheme);
        Subtheme GetSubtheme(string themeName, string subthemeName);
        IEnumerable<Subtheme> GetAllSubthemes();
        IEnumerable<Subtheme> GetAllSubthemesForYear(ushort year);
        IEnumerable<Subtheme> GetAllSubthemesForTheme(string themeName);
    }
}