using System;
using System.Collections.Generic;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IBricksetUserRepository
    {
        BricksetUser Add(BricksetUserType userType, string username);
        BricksetUser Get(string username);
        bool Exists(string username);
        bool Remove(string username);
        BricksetUserSet AddOrUpdateSet(string username, BricksetUserSet bricksetUserSet);
        BricksetUserSet GetSet(string username, long setId);
        IEnumerable<string> GetAllUsernames(BricksetUserType userType);
        BricksetUser UpdateUserSynchronizationTimestamp(string username, DateTimeOffset userSynchronizationTimestamp);
        IEnumerable<BricksetUserSet> GetWantedSets(string username);
        IEnumerable<BricksetUserSet> GetOwnedSets(string username);
    }
}
