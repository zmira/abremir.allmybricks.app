using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IBricksetUserRepository
    {
        Task<BricksetUser> Add(BricksetUserType userType, string username);
        Task<BricksetUser> Get(string username);
        Task<bool> Exists(string username);
        Task<bool> Remove(string username);
        Task<BricksetUserSet> AddOrUpdateSet(string username, BricksetUserSet bricksetUserSet);
        Task<BricksetUserSet> GetSet(string username, long setId);
        Task<IEnumerable<string>> GetAllUsernames(BricksetUserType userType);
        Task<BricksetUser> UpdateUserSynchronizationTimestamp(string username, DateTimeOffset userSynchronizationTimestamp);
        Task<IEnumerable<BricksetUserSet>> GetWantedSets(string username);
        Task<IEnumerable<BricksetUserSet>> GetOwnedSets(string username);
    }
}
