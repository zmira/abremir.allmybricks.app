using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Shared;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class InsightsRepositoryTests : DataTestsBase
    {
        private static InsightsRepository _insightsRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _insightsRepository = new InsightsRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public async Task GetDataSynchronizationTimestamp_InsightsDoesNotExist_ReturnsNull()
        {
            var timestamp = await _insightsRepository.GetDataSynchronizationTimestamp();

            Check.That(timestamp).IsNull();
        }

        [TestMethod]
        public async Task GetDataSynchronizationTimestamp_InsightsExists_ReturnsTimestamp()
        {
            var insights = new Insights
            {
                DataSynchronizationTimestamp = DateTimeOffset.Now.AddHours(-5)
            };

            await InsertData(insights);

            var timestamp = await _insightsRepository.GetDataSynchronizationTimestamp();

            Check.That(timestamp).IsEqualTo(insights.DataSynchronizationTimestamp);
        }

        [TestMethod]
        public async Task UpdateDataSynchronizationTimestamp_InsightsDoesNotExist_CreatedInsightsWithNewTimestamp()
        {
            var dataSynchronizationTimestamp = DateTimeOffset.Now.AddHours(-5);

            await _insightsRepository.UpdateDataSynchronizationTimestamp(dataSynchronizationTimestamp);

            var timestamp = await _insightsRepository.GetDataSynchronizationTimestamp();

            Check.That(timestamp).IsEqualTo(dataSynchronizationTimestamp.ToHundredthOfSecond());
        }

        [TestMethod]
        public async Task UpdateDataSynchronizationTimestamp_InsightsExists_UpdateTimestampInExistingInsights()
        {
            var dataSynchronizationTimestamp = DateTimeOffset.Now.AddHours(-5);

            var insights = new Insights
            {
                DataSynchronizationTimestamp = dataSynchronizationTimestamp
            };

            await InsertData(insights);

            dataSynchronizationTimestamp = dataSynchronizationTimestamp.AddHours(3);

            await _insightsRepository.UpdateDataSynchronizationTimestamp(dataSynchronizationTimestamp);

            var timestamp = await _insightsRepository.GetDataSynchronizationTimestamp();

            Check.That(timestamp).IsEqualTo(dataSynchronizationTimestamp.ToHundredthOfSecond());
        }
    }
}
