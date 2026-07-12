using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IUserSynchronizationService
    {
        Task<int> SynchronizeBricksetPrimaryUsersSets(string username = null);
        Task<int> SynchronizeBricksetFriendsSets(string username = null);
    }
}
