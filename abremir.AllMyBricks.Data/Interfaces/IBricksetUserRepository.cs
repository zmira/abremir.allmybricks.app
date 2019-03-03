using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IBricksetUserRepository
    {
        BricksetUser Add(string username, BricksetUserTypeEnum userType);
        BricksetUser Get(string username);
        BricksetUserSet AddOrUpdateSet(string username, BricksetUserSet bricksetUserSet);
        BricksetUserSet GetSet(string username, long setId);
    }
}
