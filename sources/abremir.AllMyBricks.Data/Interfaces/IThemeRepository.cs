using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IThemeRepository
    {
        Task<Theme> AddOrUpdate(Theme theme);
        Task<Theme> Get(string themeName);
        Task<IEnumerable<Theme>> All();
        Task<IEnumerable<Theme>> AllForYear(short year);
        Task<int> DeleteMany(List<string> themeNames);
        Task<int> Count();
    }
}
