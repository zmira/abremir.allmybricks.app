using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface IUserSynchronizer
    {
        Task SynchronizeBricksetPrimaryUser(string apiKey, string username, string userHash);
        Task SynchronizeBricksetFriend(string apiKey, string username);
    }
}
