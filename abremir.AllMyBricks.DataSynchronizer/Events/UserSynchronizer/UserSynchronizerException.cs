using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer
{
    public class UserSynchronizerException
    {
        public string PrimaryUserUsername { get; set; }
        public string FriendUsername { get; set; }
        public Exception Exception { get; set; }
    }
}
