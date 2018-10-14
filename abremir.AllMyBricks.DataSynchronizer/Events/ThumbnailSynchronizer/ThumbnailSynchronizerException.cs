using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer
{
    public class ThumbnailSynchronizerException : IDataSynchronizerEvent
    {
        public Exception Exception { get; set; }
    }
}