using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService
{
    public class ProcessingThemeException
    {
        public string Name { get; set; }
        public Exception Exception { get; set; }
    }
}