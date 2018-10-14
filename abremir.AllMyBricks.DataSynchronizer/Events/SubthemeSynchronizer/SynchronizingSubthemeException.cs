using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SynchronizingSubthemeException : IDataSynchronizerEvent
    {
        public string ThemeName { get; set; }
        public string Name { get; set; }
        public Exception Exception { get; set; }
    }
}