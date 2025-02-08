﻿using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class InsightsRepository(IRepositoryService repositoryService) : IInsightsRepository
    {
        private readonly IRepositoryService _repositoryService = repositoryService;

        public async Task<DateTimeOffset?> GetDataSynchronizationTimestamp()
        {
            return (await GetInsights().ConfigureAwait(false))?.DataSynchronizationTimestamp;
        }

        public async Task UpdateDataSynchronizationTimestamp(DateTimeOffset dataSynchronizationTimestamp)
        {
            var insights = await GetInsights().ConfigureAwait(false) ?? new Insights { Id = 1 };
            insights.DataSynchronizationTimestamp = dataSynchronizationTimestamp;

            using var repository = _repositoryService.GetRepository();

            await repository.UpsertAsync(insights).ConfigureAwait(false);
        }

        private async Task<Insights> GetInsights()
        {
            using var repository = _repositoryService.GetRepository();

            return await repository.FirstOrDefaultAsync<Insights>("1 = 1").ConfigureAwait(false);
        }
    }
}
