using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SetSynchronizerException : IDataSynchronizerEvent
    {
        public Exception Exception { get; set; }
    }
}