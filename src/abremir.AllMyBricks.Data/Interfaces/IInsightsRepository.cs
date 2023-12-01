using System;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IInsightsRepository
    {
        DateTimeOffset? GetDataSynchronizationTimestamp();
        void UpdateDataSynchronizationTimestamp(DateTimeOffset dataSynchronizationTimestamp);
    }
}
