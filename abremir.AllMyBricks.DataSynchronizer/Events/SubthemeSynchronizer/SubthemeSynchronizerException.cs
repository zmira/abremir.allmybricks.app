using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SubthemeSynchronizerException : IDataSynchronizerEvent
    {
        public Exception Exception { get; set; }
    }
}