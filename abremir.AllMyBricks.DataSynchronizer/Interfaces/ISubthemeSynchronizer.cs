using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISubthemeSynchronizer
    {
        Task<IEnumerable<Subtheme>> Synchronize(string apiKey, Theme theme);
    }
}