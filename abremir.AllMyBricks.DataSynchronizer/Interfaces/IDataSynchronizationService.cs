using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IDataSynchronizationService
    {
        Task SynchronizeAllSetData();
    }
}