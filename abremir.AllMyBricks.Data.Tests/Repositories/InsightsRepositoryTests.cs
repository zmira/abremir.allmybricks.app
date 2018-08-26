using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models.Realm;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class InsightsRepositoryTests : DataTestsBase
    {
        private static IInsightsRepository _insightsRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _insightsRepository = new InsightsRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public void GetDataSynchronizationTimestamp_InsightsDoesNotExist_ReturnsNull()
        {
            var timestamp = _insightsRepository.GetDataSynchronizationTimestamp();

            timestamp.Should().BeNull();
        }

        [TestMethod]
        public void GetDataSynchronizationTimestamp_InsightsExists_ReturnsTimestamp()
        {
            var insights = new Insights
            {
                DataSynchronizationTimestamp = DateTimeOffset.Now.AddHours(-5)
            };

            InsertData(insights);

            var timestamp = _insightsRepository.GetDataSynchronizationTimestamp();

            timestamp.Should().Be(insights.DataSynchronizationTimestamp);
        }

        [TestMethod]
        public void UpdateDataSynchronizationTimestamp_InsightsDoesNotExist_CreatedInsightsWithNewTimestamp()
        {
            var dataSynchronizationTimestamp = DateTimeOffset.Now.AddHours(-5);

            _insightsRepository.UpdateDataSynchronizationTimestamp(dataSynchronizationTimestamp);

            var timestamp = _insightsRepository.GetDataSynchronizationTimestamp();

            timestamp.Should().Be(dataSynchronizationTimestamp);
        }

        [TestMethod]
        public void UpdateDataSynchronizationTimestamp_InsightsExists_UpdateTimestampInExistingInsights()
        {
            var dataSynchronizationTimestamp = DateTimeOffset.Now.AddHours(-5);

            var insights = new Insights
            {
                DataSynchronizationTimestamp = dataSynchronizationTimestamp
            };

            InsertData(insights);

            dataSynchronizationTimestamp = dataSynchronizationTimestamp.AddHours(3);

            _insightsRepository.UpdateDataSynchronizationTimestamp(dataSynchronizationTimestamp);

            var timestamp = _insightsRepository.GetDataSynchronizationTimestamp();

            timestamp.Should().Be(dataSynchronizationTimestamp);
        }
    }
}