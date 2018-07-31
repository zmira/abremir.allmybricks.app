using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IThemeSynchronizer
    {
        IEnumerable<Theme> Synchronize(string apiKey);
    }
}