using abremir.AllMyBricks.Data.Models;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IThumbnailSynchronizer
    {
        Task Synchronize(Set set, bool requestFromSynchronizer = false);
    }
}