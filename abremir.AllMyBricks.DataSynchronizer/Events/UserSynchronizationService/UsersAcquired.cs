using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService
{
    public class UsersAcquired
    {
        public BricksetUserTypeEnum UserType { get; set; }
        public int Count { get; set; }
    }
}
