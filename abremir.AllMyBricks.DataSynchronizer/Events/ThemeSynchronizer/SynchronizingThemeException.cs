using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer
{
    public class SynchronizingThemeException : SynchronizingTheme
    {
        public Exception Exception { get; set; }
    }
}