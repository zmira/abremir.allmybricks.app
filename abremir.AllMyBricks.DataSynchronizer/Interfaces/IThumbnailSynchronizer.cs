using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IThumbnailSynchronizer
    {
        void Synchronize(Set set, bool requestFromSynchronizer = false);
    }
}