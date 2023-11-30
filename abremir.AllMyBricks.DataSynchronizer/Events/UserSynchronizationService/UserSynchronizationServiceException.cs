using System;
using System.Collections.Generic;
using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService
{
    public class UserSynchronizationServiceException
    {
        public BricksetUserType UserType { get; set; }
        public IEnumerable<Exception> Exceptions { get; set; }
    }
}
