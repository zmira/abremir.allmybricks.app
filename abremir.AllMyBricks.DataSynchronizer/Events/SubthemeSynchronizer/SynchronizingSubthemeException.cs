using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SynchronizingSubthemeException : SynchronizingSubtheme
    {
        public Exception Exception { get; set; }
    }
}