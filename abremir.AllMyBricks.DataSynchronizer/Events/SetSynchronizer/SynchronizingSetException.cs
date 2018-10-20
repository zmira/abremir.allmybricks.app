using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SynchronizingSetException : SynchronizingSet
    {
        public Exception Exception { get; set; }
    }
}