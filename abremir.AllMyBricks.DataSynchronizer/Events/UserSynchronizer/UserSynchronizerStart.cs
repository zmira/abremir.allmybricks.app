using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer
{
    public class UserSynchronizerStart
    {
        public BricksetUserType UserType { get; set; }
        public string Username { get; set; }
    }
}
