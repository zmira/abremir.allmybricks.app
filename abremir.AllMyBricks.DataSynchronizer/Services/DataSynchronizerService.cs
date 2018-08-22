using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class DataSynchronizerService : IDataSynchronizerService
    {
        private readonly IThemeSynchronizer _themeSynchronizer;
        private readonly ISubthemeSynchronizer _subthemeSynchronizer;
        private readonly ISetSynchronizer _setSynchronizer;
        private readonly IInsightsRepository _insightsRepository;
        private readonly IOnboardingService _onboardingService;
        private readonly IPreferencesService _preferencesService;

        public DataSynchronizerService(
            IThemeSynchronizer themeSynchronizer,
            ISubthemeSynchronizer subthemeSynchronizer,
            ISetSynchronizer setSynchronizer,
            IInsightsRepository insightsRepository,
            IOnboardingService onboardingService,
            IPreferencesService preferencesService)
        {
            _themeSynchronizer = themeSynchronizer;
            _subthemeSynchronizer = subthemeSynchronizer;
            _setSynchronizer = setSynchronizer;
            _insightsRepository = insightsRepository;
            _onboardingService = onboardingService;
            _preferencesService = preferencesService;
        }

        public void SynchronizeAllSetData()
        {
            var apiKey = _onboardingService.GetBricksetApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return;
            }

            var dataSynchronizationTimestamp = _insightsRepository.GetDataSynchronizationTimestamp();

            var synchronizedSets = new List<Set>();

            foreach (var theme in _themeSynchronizer.Synchronize(apiKey))
            {
                var subthemes = _subthemeSynchronizer.Synchronize(apiKey, theme);

                if (!dataSynchronizationTimestamp.HasValue)
                {
                    foreach (var subtheme in subthemes)
                    {
                        synchronizedSets.AddRange(_setSynchronizer.Synchronize(apiKey, theme, subtheme));
                    }

                    _insightsRepository.UpdateDataSynchronizationTimestamp(DateTimeOffset.Now);
                }
            }

            if (dataSynchronizationTimestamp.HasValue)
            {
                synchronizedSets.AddRange(_setSynchronizer.Synchronize(apiKey, dataSynchronizationTimestamp.Value));
                _insightsRepository.UpdateDataSynchronizationTimestamp(DateTimeOffset.Now);
            }

            if (_preferencesService.RetrieveFullSetDataOnSynchronization)
            {
                foreach (var set in synchronizedSets)
                {
                    _setSynchronizer.Synchronize(apiKey, set.SetId);
                }
            }
        }
    }
}