using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IThumbnailSynchronizer
    {
        Task Synchronize(Set set, bool requestFromSynchronizer = false);
    }
}
