using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using System;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class InsightsRepository : IInsightsRepository
    {
        private readonly IRepositoryService _repositoryService;

        public InsightsRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public DateTimeOffset? GetDataSynchronizationTimestamp()
        {
            return GetInsights()?.DataSynchronizationTimestamp;
        }

        public void UpdateDataSynchronizationTimestamp(DateTimeOffset dataSynchronizationTimestamp)
        {
            var insights = GetInsights() ?? new Insights { Id = 1 };
            insights.DataSynchronizationTimestamp = dataSynchronizationTimestamp.ToHundredthOfSecond();

            using var repository = _repositoryService.GetRepository();

            repository.Upsert(insights);
        }

        private Insights GetInsights()
        {
            using var repository = _repositoryService.GetRepository();

            return repository.FirstOrDefault<Insights>("1 = 1");
        }
    }
}
