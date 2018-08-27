using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class DataSynchronizationService : IDataSynchronizationService
    {
        private readonly IThemeSynchronizer _themeSynchronizer;
        private readonly ISubthemeSynchronizer _subthemeSynchronizer;
        private readonly ISetSynchronizer _setSynchronizer;
        private readonly IInsightsRepository _insightsRepository;
        private readonly IOnboardingService _onboardingService;

        public event EventHandler DataSynchronizationStart;
        public event EventHandler DataSynchronizationEnd;
        public event EventHandler<string> ProcessingTheme;
        public event EventHandler<string> ProcessedTheme;
        public event EventHandler<string> ProcessingSubtheme;
        public event EventHandler<string> ProcessedSubtheme;

        public DataSynchronizationService(
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
            DataSynchronizationStart?.Invoke(this, null);

            var apiKey = _onboardingService.GetBricksetApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return;
            }

            var dataSynchronizationTimestamp = _insightsRepository.GetDataSynchronizationTimestamp();

            foreach (var theme in _themeSynchronizer.Synchronize(apiKey))
            {
                ProcessingTheme?.Invoke(this, theme.Name);

                var subthemes = _subthemeSynchronizer.Synchronize(apiKey, theme);

                if (!dataSynchronizationTimestamp.HasValue)
                {
                    foreach (var subtheme in subthemes)
                    {
                        ProcessingSubtheme?.Invoke(this, subtheme.Name);

                        _setSynchronizer.Synchronize(apiKey, theme, subtheme);

                        ProcessedSubtheme?.Invoke(this, subtheme.Name);
                    }

                    _insightsRepository.UpdateDataSynchronizationTimestamp(DateTimeOffset.Now);
                }

                ProcessedTheme?.Invoke(this, theme.Name);
            }

            if (dataSynchronizationTimestamp.HasValue)
            {
                _setSynchronizer.Synchronize(apiKey, dataSynchronizationTimestamp.Value);
                _insightsRepository.UpdateDataSynchronizationTimestamp(DateTimeOffset.Now);
            }

            DataSynchronizationEnd?.Invoke(this, null);
        }
    }
}