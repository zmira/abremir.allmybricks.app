using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SynchronizingSetException : IDataSynchronizerEvent
    {
        public string Identifier { get; set; }
        public Exception Exception { get; set; }
    }
}