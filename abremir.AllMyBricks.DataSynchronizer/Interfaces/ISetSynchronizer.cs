using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISetSynchronizer
    {
        bool Synchronize(string apiKey, Theme theme, Subtheme subtheme);
    }
}