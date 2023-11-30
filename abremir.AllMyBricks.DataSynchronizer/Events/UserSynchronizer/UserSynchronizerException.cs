using System;
using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer
{
    public class UserSynchronizerException
    {
        public BricksetUserType UserType { get; set; }
        public string Username { get; set; }
        public Exception Exception { get; set; }
    }
}
