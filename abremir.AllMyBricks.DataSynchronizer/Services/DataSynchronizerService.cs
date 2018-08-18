using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
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

        public DataSynchronizerService(
            IThemeSynchronizer themeSynchronizer,
            ISubthemeSynchronizer subthemeSynchronizer,
            ISetSynchronizer setSynchronizer,
            IInsightsRepository insightsRepository,
            IOnboardingService onboardingService)
        {
            _themeSynchronizer = themeSynchronizer;
            _subthemeSynchronizer = subthemeSynchronizer;
            _setSynchronizer = setSynchronizer;
            _insightsRepository = insightsRepository;
            _onboardingService = onboardingService;
        }

        public void SynchronizeAllSetData()
        {
            var apiKey = _onboardingService.GetBricksetApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return;
            }

            var dataSynchronizationTimestamp = _insightsRepository.GetDataSynchronizationTimestamp();

            foreach (var theme in _themeSynchronizer.Synchronize(apiKey))
            {
                var subthemes = _subthemeSynchronizer.Synchronize(apiKey, theme);

                if (!dataSynchronizationTimestamp.HasValue)
                {
                    foreach (var subtheme in subthemes)
                    {
                        _setSynchronizer.Synchronize(apiKey, theme, subtheme);
                    }

                    _insightsRepository.UpdateDataSynchronizationTimestamp(DateTimeOffset.Now);
                }
            }

            if (dataSynchronizationTimestamp.HasValue)
            {
                _setSynchronizer.Synchronize(apiKey, dataSynchronizationTimestamp.Value);
                _insightsRepository.UpdateDataSynchronizationTimestamp(DateTimeOffset.Now);
            }
        }
    }
}