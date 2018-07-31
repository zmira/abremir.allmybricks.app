using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISubthemeSynchronizer
    {
        IEnumerable<Subtheme> Synchronize(string apiKey, Theme theme);
    }
}