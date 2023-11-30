using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IThemeSynchronizer
    {
        Task<IEnumerable<Theme>> Synchronize(string apiKey);
    }
}
