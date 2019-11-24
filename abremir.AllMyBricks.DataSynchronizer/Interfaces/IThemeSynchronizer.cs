using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IThemeSynchronizer
    {
        Task<IEnumerable<Theme>> Synchronize(string apiKey);
    }
}
