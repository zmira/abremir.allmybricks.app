using abremir.AllMyBricks.Data.Enumerations;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService
{
    public class UserSynchronizationServiceException
    {
        public BricksetUserTypeEnum UserType { get; set; }
        public IEnumerable<Exception> Exceptions { get; set; }
    }
}
