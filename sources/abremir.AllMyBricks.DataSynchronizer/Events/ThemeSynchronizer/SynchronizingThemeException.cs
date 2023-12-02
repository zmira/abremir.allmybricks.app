using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer
{
    public class SynchronizingThemeException
    {
        public string Theme { get; set; }
        public Exception Exception { get; set; }
    }
}
