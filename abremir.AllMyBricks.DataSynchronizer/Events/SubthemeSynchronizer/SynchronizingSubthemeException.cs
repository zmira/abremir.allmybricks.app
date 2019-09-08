using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SynchronizingSubthemeException
    {
        public string Theme { get; set; }
        public string Subtheme { get; set; }
        public Exception Exception { get; set; }
    }
}