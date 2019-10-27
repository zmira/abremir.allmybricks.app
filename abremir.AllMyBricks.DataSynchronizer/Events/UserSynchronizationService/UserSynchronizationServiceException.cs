using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService
{
    public class UserSynchronizationServiceException
    {
        public IEnumerable<Exception> Exceptions { get; set; }
    }
}
