using System;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IInsightsRepository
    {
        Task<DateTimeOffset?> GetDataSynchronizationTimestamp();
        Task UpdateDataSynchronizationTimestamp(DateTimeOffset dataSynchronizationTimestamp);
    }
}
