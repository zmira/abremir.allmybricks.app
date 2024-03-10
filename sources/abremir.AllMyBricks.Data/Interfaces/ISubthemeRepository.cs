using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface ISubthemeRepository
    {
        Task<Subtheme> AddOrUpdate(Subtheme subtheme);
        Task<Subtheme> Get(string themeName, string subthemeName);
        Task<IEnumerable<Subtheme>> All();
        Task<IEnumerable<Subtheme>> AllForYear(short year);
        Task<IEnumerable<Subtheme>> AllForTheme(string themeName);
    }
}
