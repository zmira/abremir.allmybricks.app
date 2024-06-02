using System;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThemeSynchronizer(
        IOnboardingService onboardingService,
        IBricksetApiService bricksetService,
        IThemeRepository themeRepository,
        IMessageHub messageHub) : IThemeSynchronizer
    {
        private readonly IOnboardingService _onboardingService = onboardingService;
        private readonly IBricksetApiService _bricksetApiService = bricksetService;
        private readonly IThemeRepository _themeRepository = themeRepository;
        private readonly IMessageHub _messageHub = messageHub;

        public async Task Synchronize()
        {
            _messageHub.Publish(new ThemeSynchronizerStart());

            var apiKey = await _onboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                _messageHub.Publish(new ThemeSynchronizerException { Exception = exception });

                throw exception;
            }

            try
            {
                var getThemesParameters = new ParameterApiKey
                {
                    ApiKey = apiKey
                };

                var bricksetThemes = (await _bricksetApiService.GetThemes(getThemesParameters).ConfigureAwait(false)).ToList();

                _messageHub.Publish(new ThemesAcquired { Count = bricksetThemes.Count });

                foreach (var bricksetTheme in bricksetThemes)
                {
                    _messageHub.Publish(new SynchronizingThemeStart { Theme = bricksetTheme.Theme });

                    var theme = bricksetTheme.ToTheme();

                    var getYearsParameters = new ParameterTheme
                    {
                        ApiKey = apiKey,
                        Theme = bricksetTheme.Theme
                    };

                    theme.SetCountPerYear = (await _bricksetApiService.GetYears(getYearsParameters).ConfigureAwait(false))
                        .ToYearSetCountEnumerable()
                        .ToList();

                    var persistedTheme = await _themeRepository.Get(theme.Name).ConfigureAwait(false);

                    if (persistedTheme != null)
                    {
                        theme.Id = persistedTheme.Id;
                    }

                    await _themeRepository.AddOrUpdate(theme).ConfigureAwait(false);

                    _messageHub.Publish(new SynchronizingThemeEnd { Theme = bricksetTheme.Theme });
                }
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new ThemeSynchronizerException { Exception = ex });

                throw;
            }

            _messageHub.Publish(new ThemeSynchronizerEnd());
        }
    }
}
