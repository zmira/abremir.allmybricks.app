using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService
{
    public class InsightsAcquired : IDataSynchronizerEvent
    {
        public DateTimeOffset? SynchronizationTimestamp { get; set; }
    }
}
