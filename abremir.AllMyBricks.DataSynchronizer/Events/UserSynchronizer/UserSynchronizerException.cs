using abremir.AllMyBricks.Data.Enumerations;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer
{
    public class UserSynchronizerException
    {
        public BricksetUserType UserType { get; set; }
        public string Username { get; set; }
        public Exception Exception { get; set; }
    }
}
