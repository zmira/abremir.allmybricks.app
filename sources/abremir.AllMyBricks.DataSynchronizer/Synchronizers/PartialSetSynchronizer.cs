﻿using System;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class PartialSetSynchronizer : SetSynchronizerBase, IPartialSetSynchronizer
    {
        public PartialSetSynchronizer(
            IInsightsRepository insightsRepository,
            IOnboardingService onboardingService,
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository,
            IThumbnailSynchronizer thumbnailSynchronizer,
            IMessageHub messageHub)
            : base(insightsRepository, onboardingService, bricksetApiService, setRepository, referenceDataRepository, themeRepository, subthemeRepository, thumbnailSynchronizer, messageHub) { }

        public async Task Synchronize()
        {
            MessageHub.Publish(new SetSynchronizerStart { Complete = false });

            var dataSynchronizationTimestamp = InsightsRepository.GetDataSynchronizationTimestamp();

            MessageHub.Publish(new InsightsAcquired { SynchronizationTimestamp = dataSynchronizationTimestamp });

            if (!dataSynchronizationTimestamp.HasValue)
            {
                MessageHub.Publish(new SetSynchronizerEnd { Complete = false });

                return;
            }

            var previousUpdateTimestamp = dataSynchronizationTimestamp.Value;

            var apiKey = await OnboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                MessageHub.Publish(new ThemeSynchronizerException { Exception = exception });

                throw exception;
            }

            try
            {
                MessageHub.Publish(new AcquiringSetsStart { Complete = false, From = previousUpdateTimestamp });

                var getSetsParameters = new GetSetsParameters
                {
                    UpdatedSince = previousUpdateTimestamp.UtcDateTime
                };

                var updatedSets = await GetAllSetsFor(apiKey, getSetsParameters);
                var newUpdateTimestamp = DateTimeOffset.UtcNow;

                MessageHub.Publish(new AcquiringSetsEnd { Count = updatedSets.Count, Complete = false, From = previousUpdateTimestamp });

                foreach (var themeGroup in updatedSets.GroupBy(bricksetSet => bricksetSet.Theme))
                {
                    var theme = ThemeRepository.Get(themeGroup.Key);

                    foreach (var subthemeGroup in themeGroup.GroupBy(themeSets => themeSets.Subtheme))
                    {
                        var subtheme = SubthemeRepository.Get(theme.Name, subthemeGroup.Key);

                        foreach (var bricksetSet in subthemeGroup)
                        {
                            await AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet);
                        }
                    }
                }

                InsightsRepository.UpdateDataSynchronizationTimestamp(newUpdateTimestamp);
            }
            catch (Exception ex)
            {
                MessageHub.Publish(new SetSynchronizerException { Exception = ex });

                throw;
            }

            MessageHub.Publish(new SetSynchronizerEnd { Complete = false });
        }
    }
}
