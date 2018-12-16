using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using System;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class DataSynchronizationService : IDataSynchronizationService
    {
        private readonly IThemeSynchronizer _themeSynchronizer;
        private readonly ISubthemeSynchronizer _subthemeSynchronizer;
        private readonly ISetSynchronizer _setSynchronizer;
        private readonly IInsightsRepository _insightsRepository;
        private readonly IOnboardingService _onboardingService;
        private readonly IDataSynchronizerEventManager _dataSynchronizerEventHandler;

        public DataSynchronizationService(
            IThemeSynchronizer themeSynchronizer,
            ISubthemeSynchronizer subthemeSynchronizer,
            ISetSynchronizer setSynchronizer,
            IInsightsRepository insightsRepository,
            IOnboardingService onboardingService,
            IDataSynchronizerEventManager dataSynchronizerEventHandler)
        {
            _themeSynchronizer = themeSynchronizer;
            _subthemeSynchronizer = subthemeSynchronizer;
            _setSynchronizer = setSynchronizer;
            _insightsRepository = insightsRepository;
            _onboardingService = onboardingService;
            _dataSynchronizerEventHandler = dataSynchronizerEventHandler;
        }

        public async Task SynchronizeAllSetData()
        {
            _dataSynchronizerEventHandler.Raise(new DataSynchronizationStart());

            try
            {
                var apiKey = await _onboardingService.GetBricksetApiKey();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return;
                }

                var dataSynchronizationTimestamp = _insightsRepository.GetDataSynchronizationTimestamp();

                _dataSynchronizerEventHandler.Raise(new InsightsAcquired { SynchronizationTimestamp = dataSynchronizationTimestamp });

                foreach (var theme in await _themeSynchronizer.Synchronize(apiKey))
                {
                    _dataSynchronizerEventHandler.Raise(new ProcessingTheme { Name = theme.Name });

                    try
                    {
                        var subthemes = await _subthemeSynchronizer.Synchronize(apiKey, theme);

                        if (!dataSynchronizationTimestamp.HasValue)
                        {
                            foreach (var subtheme in subthemes)
                            {
                                _dataSynchronizerEventHandler.Raise(new ProcessingSubtheme { Name = subtheme.Name });

                                await _setSynchronizer.Synchronize(apiKey, theme, subtheme);

                                _dataSynchronizerEventHandler.Raise(new ProcessedSubtheme { Name = subtheme.Name });
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        _dataSynchronizerEventHandler.Raise(new ProcessingThemeException { Name = theme.Name, Exception = ex });
                    }

                    _dataSynchronizerEventHandler.Raise(new ProcessedTheme { Name = theme.Name });
                }

                if (dataSynchronizationTimestamp.HasValue)
                {
                    await _setSynchronizer.Synchronize(apiKey, dataSynchronizationTimestamp.Value);
                }

                _insightsRepository.UpdateDataSynchronizationTimestamp(DateTimeOffset.Now);
            }
            catch(Exception ex)
            {
                _dataSynchronizerEventHandler.Raise(new DataSynchronizationException { Exception = ex });
            }

            _dataSynchronizerEventHandler.Raise(new DataSynchronizationEnd());
        }
    }
}