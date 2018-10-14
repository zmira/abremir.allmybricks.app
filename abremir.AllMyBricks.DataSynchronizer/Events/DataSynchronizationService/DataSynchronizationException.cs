using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService
{
    public class DataSynchronizationException : IDataSynchronizerEvent
    {
        public Exception Exception { get; set; }
    }
}