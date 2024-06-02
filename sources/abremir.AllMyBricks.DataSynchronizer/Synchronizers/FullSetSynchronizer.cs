using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class FullSetSynchronizer : SetSynchronizerBase, IFullSetSynchronizer
    {
        public FullSetSynchronizer(
            IInsightsRepository insightsRepository,
            IOnboardingService onboardingService,
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository,
            IBricksetUserRepository bricksetUserRepository,
            IThumbnailSynchronizer thumbnailSynchronizer,
            IMessageHub messageHub)
            : base(insightsRepository, onboardingService, bricksetApiService, setRepository, referenceDataRepository, themeRepository, subthemeRepository, bricksetUserRepository, thumbnailSynchronizer, messageHub) { }

        public async Task Synchronize()
        {
            MessageHub.Publish(new SetSynchronizerStart { Type = SetAcquisitionType.Seed });

            var dataSynchronizationTimestamp = await InsightsRepository.GetDataSynchronizationTimestamp().ConfigureAwait(false);

            MessageHub.Publish(new InsightsAcquired { SynchronizationTimestamp = dataSynchronizationTimestamp });

            if (dataSynchronizationTimestamp.HasValue)
            {
                MessageHub.Publish(new SetSynchronizerEnd { Type = SetAcquisitionType.Seed });

                return;
            }

            var apiKey = await OnboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                MessageHub.Publish(new SetSynchronizerException { Exception = exception });

                throw exception;
            }

            var yearSetCount = (await ThemeRepository
                .All().ConfigureAwait(false))
                .SelectMany(theme => theme.SetCountPerYear)
                .GroupBy(setCountPerYear => setCountPerYear.Year)
                .ToFrozenDictionary(group => group.Key, group => group.Sum(value => value.SetCount));

            List<List<string>> queries = [];
            var tempSetCount = 0;
            List<string> tempYearList = [];
            var orderedYears = yearSetCount.Keys.Order().ToList();

            for (var i = 0; i < orderedYears.Count; i++)
            {
                var year = orderedYears[i];
                if (yearSetCount[year] > Constants.BricksetMaximumPageSizeParameter)
                {
                    queries.Add([year.ToString()]);
                }
                else
                {
                    if (tempSetCount + yearSetCount[year] > Constants.BricksetMaximumPageSizeParameter)
                    {
                        queries.Add(tempYearList);

                        tempSetCount = 0;
                        tempYearList = [];
                    }

                    tempYearList.Add(year.ToString());
                    tempSetCount += yearSetCount[year];

                    if (i == orderedYears.Count - 1)
                    {
                        queries.Add(tempYearList);
                    }
                }
            }

            var newUpdateTimestamp = DateTimeOffset.UtcNow;

            try
            {
                foreach (var query in queries)
                {
                    var yearFilter = string.Join(",", query);

                    var getSetsParameters = new GetSetsParameters
                    {
                        PageSize = Constants.BricksetMaximumPageSizeParameter,
                        Year = yearFilter
                    };

                    MessageHub.Publish(new AcquiringSetsStart { Type = Enumerations.SetAcquisitionType.Seed, Parameters = getSetsParameters });

                    var bricksetSets = await GetAllSetsFor(apiKey, getSetsParameters).ConfigureAwait(false);

                    MessageHub.Publish(new AcquiringSetsEnd { Count = bricksetSets.Count, Type = Enumerations.SetAcquisitionType.Seed, Parameters = getSetsParameters });

                    foreach (var bricksetSet in bricksetSets)
                    {
                        var theme = await ThemeRepository.Get(bricksetSet.Theme).ConfigureAwait(false);
                        var subtheme = await SubthemeRepository.Get(theme.Name, bricksetSet.Subtheme).ConfigureAwait(false);

                        await AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet, (short)bricksetSet.Year).ConfigureAwait(false);
                    }
                }

                await InsightsRepository.UpdateDataSynchronizationTimestamp(newUpdateTimestamp).ConfigureAwait(false);

                var expectedTotalNumberOfSets = yearSetCount.Values.Sum();
                var actualTotalNumberOfSets = await SetRepository.Count().ConfigureAwait(false);

                if (actualTotalNumberOfSets != expectedTotalNumberOfSets)
                {
                    MessageHub.Publish(new MismatchingNumberOfSetsWarning { Expected = expectedTotalNumberOfSets, Actual = actualTotalNumberOfSets });
                }
            }
            catch (Exception ex)
            {
                MessageHub.Publish(new SetSynchronizerException { Exception = ex });

                throw;
            }

            MessageHub.Publish(new SetSynchronizerEnd { Type = SetAcquisitionType.Seed });
        }
    }
}
