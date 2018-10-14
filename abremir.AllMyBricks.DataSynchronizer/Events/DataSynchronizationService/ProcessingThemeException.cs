using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService
{
    public class ProcessingThemeException : IDataSynchronizerEvent
    {
        public string Name { get; set; }
        public Exception Exception { get; set; }
    }
}