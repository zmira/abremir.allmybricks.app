using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService
{
    public class UsersAcquired
    {
        public BricksetUserType UserType { get; set; }
        public int Count { get; set; }
    }
}
