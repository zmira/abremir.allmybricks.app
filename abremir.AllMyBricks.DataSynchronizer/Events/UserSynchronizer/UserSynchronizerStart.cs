using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer
{
    public class UserSynchronizerStart
    {
        public BricksetUserTypeEnum UserType { get; set; }
        public string Username { get; set; }
    }
}
