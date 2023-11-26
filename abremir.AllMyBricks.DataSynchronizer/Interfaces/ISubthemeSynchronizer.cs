using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISubthemeSynchronizer
    {
        Task<IEnumerable<Subtheme>> Synchronize(string apiKey, Theme theme);
    }
}
