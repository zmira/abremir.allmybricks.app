using System.Threading.Tasks;

namespace abremir.AllMyBricks.UserManagement.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddDefaultUser();
        Task<bool> AddBricksetPrimaryUser(string username, string password);
        Task<bool> RemoveBricksetPrimaryUser(string username);
        Task<bool> AddBricksetFriend(string username);
        bool RemoveBricksetFriend(string username);
    }
}
