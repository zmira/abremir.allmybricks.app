using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using abremir.AllMyBricks.Onboarding.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstituteAutoMocker.Standard;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Services
{
    [TestClass]
    public class SetSynchronizationServiceTests
    {
        private NSubstituteAutoMocker<SetSynchronizationService> _dataSynchronizationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _dataSynchronizationService = new NSubstituteAutoMocker<SetSynchronizationService>();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow(" ")]
        public async Task SynchronizeAllSets_InvalidApiKey_GetDataSynchronizationTimestampNotInvoked(string apiKey)
        {
            _dataSynchronizationService.Get<IOnboardingService>()
                .GetBricksetApiKey()
                .Returns(apiKey);

            await _dataSynchronizationService.ClassUnderTest.SynchronizeAllSets().ConfigureAwait(false);

            _dataSynchronizationService.Get<IInsightsRepository>().DidNotReceive().GetDataSynchronizationTimestamp();
        }

        [TestMethod]
        public async Task SynchronizeAllSets_WithoutDataSynchronizationTimestamp_SynchronizeAllSetsAndUpdateDataSynchronizationTimestampInvoked()
        {
            _dataSynchronizationService.Get<IOnboardingService>()
                .GetBricksetApiKey()
                .Returns("APIKEY");
            _dataSynchronizationService.Get<IInsightsRepository>()
                .GetDataSynchronizationTimestamp().
                Returns((DateTimeOffset?)null);
            _dataSynchronizationService.Get<IThemeSynchronizer>()
                .Synchronize(Arg.Any<string>())
                .Returns(new List<Theme> { new() });
            _dataSynchronizationService.Get<ISubthemeSynchronizer>()
                .Synchronize(Arg.Any<string>(), Arg.Any<Theme>())
                .Returns(new List<Subtheme> { new() });

            await _dataSynchronizationService.ClassUnderTest.SynchronizeAllSets().ConfigureAwait(false);

            await _dataSynchronizationService.Get<IThemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>()).ConfigureAwait(false);
            await _dataSynchronizationService.Get<ISubthemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>()).ConfigureAwait(false);
            await _dataSynchronizationService.Get<ISetSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>(), Arg.Any<Subtheme>()).ConfigureAwait(false);
            await _dataSynchronizationService.Get<ISetSynchronizer>().DidNotReceive().Synchronize(Arg.Any<string>(), Arg.Any<DateTimeOffset>()).ConfigureAwait(false);
            _dataSynchronizationService.Get<IInsightsRepository>().Received(1).UpdateDataSynchronizationTimestamp(Arg.Any<DateTimeOffset>());
        }

        [TestMethod]
        public async Task SynchronizeAllSets_WithDataSynchronizationTimestamp_SynchronizeRecentlyUpdatedSetsAndUpdateDataSynchronizationTimestampInvoked()
        {
            _dataSynchronizationService.Get<IOnboardingService>()
                .GetBricksetApiKey()
                .Returns("APIKEY");
            _dataSynchronizationService.Get<IInsightsRepository>()
                .GetDataSynchronizationTimestamp()
                .Returns(DateTimeOffset.Now);
            _dataSynchronizationService.Get<IThemeSynchronizer>()
                .Synchronize(Arg.Any<string>())
                .Returns(new List<Theme> { new() });
            _dataSynchronizationService.Get<ISubthemeSynchronizer>()
                .Synchronize(Arg.Any<string>(), Arg.Any<Theme>())
                .Returns(new List<Subtheme> { new() });

            await _dataSynchronizationService.ClassUnderTest.SynchronizeAllSets().ConfigureAwait(false);

            await _dataSynchronizationService.Get<IThemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>()).ConfigureAwait(false);
            await _dataSynchronizationService.Get<ISubthemeSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<Theme>()).ConfigureAwait(false);
            await _dataSynchronizationService.Get<ISetSynchronizer>().DidNotReceive().Synchronize(Arg.Any<string>(), Arg.Any<Theme>(), Arg.Any<Subtheme>()).ConfigureAwait(false);
            await _dataSynchronizationService.Get<ISetSynchronizer>().Received(1).Synchronize(Arg.Any<string>(), Arg.Any<DateTimeOffset>()).ConfigureAwait(false);
            _dataSynchronizationService.Get<IInsightsRepository>().Received(1).UpdateDataSynchronizationTimestamp(Arg.Any<DateTimeOffset>());
        }
    }
}
