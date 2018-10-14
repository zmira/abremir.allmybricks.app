using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer
{
    public class ThemeSynchronizerException : IDataSynchronizerEvent
    {
        public Exception Exception { get; set; }
    }
}