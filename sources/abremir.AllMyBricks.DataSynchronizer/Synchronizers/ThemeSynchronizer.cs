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
        IMessageHub messageHub)
        : IThemeSynchronizer
    {
        public async Task Synchronize()
        {
            messageHub.Publish(new ThemeSynchronizerStart());

            var apiKey = await onboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                messageHub.Publish(new ThemeSynchronizerException { Exception = exception });

                throw exception;
            }

            try
            {
                var getThemesParameters = new ParameterApiKey
                {
                    ApiKey = apiKey
                };

                var bricksetThemes = (await bricksetService.GetThemes(getThemesParameters).ConfigureAwait(false)).ToList();

                messageHub.Publish(new ThemesAcquired { Count = bricksetThemes.Count });

                foreach (var bricksetTheme in bricksetThemes)
                {
                    messageHub.Publish(new SynchronizingThemeStart { Theme = bricksetTheme.Theme });

                    var theme = bricksetTheme.ToTheme();

                    var getYearsParameters = new ParameterTheme
                    {
                        ApiKey = apiKey,
                        Theme = bricksetTheme.Theme
                    };

                    theme.SetCountPerYear = (await bricksetService.GetYears(getYearsParameters).ConfigureAwait(false))
                        .ToYearSetCountEnumerable()
                        .ToList();

                    var persistedTheme = await themeRepository.Get(theme.Name).ConfigureAwait(false);

                    if (persistedTheme != null)
                    {
                        theme.Id = persistedTheme.Id;
                    }

                    await themeRepository.AddOrUpdate(theme).ConfigureAwait(false);

                    messageHub.Publish(new SynchronizingThemeEnd { Theme = bricksetTheme.Theme });
                }
            }
            catch (Exception ex)
            {
                messageHub.Publish(new ThemeSynchronizerException { Exception = ex });

                throw;
            }

            messageHub.Publish(new ThemeSynchronizerEnd());
        }
    }
}
