using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer
{
    public class SynchronizingThemeException : IDataSynchronizerEvent
    {
        public string Name { get; set; }
        public Exception Exception { get; set; }
    }
}