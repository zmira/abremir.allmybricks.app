using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models.Realm;
using System;
using System.Linq;

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
            var repository = _repositoryService.GetRepository();
            var insights = GetInsights();

            if (insights == null)
            {
                repository.Write(() =>
                {
                    repository.Add(new Insights
                    {
                        DataSynchronizationTimestamp = dataSynchronizationTimestamp
                    });
                });
            }
            else
            {
                using (var transaction = repository.BeginWrite())
                {
                    insights.DataSynchronizationTimestamp = dataSynchronizationTimestamp;

                    transaction.Commit();
                }
            }
        }

        private Insights GetInsights()
        {
            return _repositoryService
                .GetRepository()
                .All<Insights>()
                .SingleOrDefault();
        }
    }
}
