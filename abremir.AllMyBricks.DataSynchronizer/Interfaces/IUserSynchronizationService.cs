using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IUserSynchronizationService
    {
        Task SynchronizeBricksetPrimaryUsersSets(string username = null);
        Task SynchronizeBricksetFriendsSets(string username = null);
    }
}
