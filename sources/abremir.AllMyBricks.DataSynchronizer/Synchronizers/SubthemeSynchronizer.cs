using System;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class SubthemeSynchronizer(
        IOnboardingService onboardingService,
        IBricksetApiService bricksetApiService,
        IThemeRepository themeRepository,
        ISubthemeRepository subthemeRepository,
        IMessageHub messageHub)
        : ISubthemeSynchronizer
    {
        public async Task Synchronize()
        {
            messageHub.Publish(new SubthemeSynchronizerStart());

            var apiKey = await onboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                messageHub.Publish(new SubthemeSynchronizerException { Exception = exception });

                throw exception;
            }

            try
            {
                var themes = await themeRepository.All().ConfigureAwait(false);

                messageHub.Publish(new ThemesAcquired { Count = themes.Count() });

                foreach (var theme in themes)
                {
                    var getSubthemesParameters = new ParameterTheme
                    {
                        ApiKey = apiKey,
                        Theme = theme.Name
                    };

                    var bricksetSubthemes = (await bricksetApiService.GetSubthemes(getSubthemesParameters).ConfigureAwait(false)).ToList();

                    messageHub.Publish(new SubthemesAcquired { Theme = theme.Name, Count = bricksetSubthemes.Count });

                    foreach (var bricksetSubtheme in bricksetSubthemes)
                    {
                        messageHub.Publish(new SynchronizingSubthemeStart { Theme = theme.Name, Subtheme = bricksetSubtheme.Subtheme });

                        var subtheme = bricksetSubtheme.ToSubtheme();

                        subtheme.Theme = theme;

                        var persistedSubtheme = await subthemeRepository.Get(subtheme.Theme.Name, subtheme.Name).ConfigureAwait(false);

                        if (persistedSubtheme != null)
                        {
                            subtheme.Id = persistedSubtheme.Id;
                        }

                        await subthemeRepository.AddOrUpdate(subtheme).ConfigureAwait(false);

                        messageHub.Publish(new SynchronizingSubthemeEnd { Theme = theme.Name, Subtheme = bricksetSubtheme.Subtheme });
                    }
                }
            }
            catch (Exception ex)
            {
                messageHub.Publish(new SubthemeSynchronizerException { Exception = ex });

                throw;
            }

            messageHub.Publish(new SubthemeSynchronizerEnd());
        }
    }
}
