using System;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSanitizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThemeSanitizer : SetSynchronizerBase, IThemeSanitizer
    {
        public ThemeSanitizer(
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
            MessageHub.Publish(new ThemeSanitizerStart());

            var apiKey = await OnboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                MessageHub.Publish(new ThemeSanitizerException { Exception = exception });

                throw exception;
            }

            var (ExpectedNumberOfSets, ActualNumberOfSets) = await GetSetNumbers();

            if (ActualNumberOfSets == ExpectedNumberOfSets)
            {
                MessageHub.Publish(new ThemeSanitizerEnd());

                return;
            }

            MessageHub.Publish(new MismatchingNumberOfSetsWarning { Expected = ExpectedNumberOfSets, Actual = ActualNumberOfSets });

            var getThemesParameters = new ParameterApiKey
            {
                ApiKey = apiKey
            };

            var bricksetThemes = (await BricksetApiService.GetThemes(getThemesParameters).ConfigureAwait(false))
                .Select(theme => theme.Theme)
                .Order()
                .ToList();

            MessageHub.Publish(new ThemesAcquired { Count = bricksetThemes.Count });

            var allMyBricksThemes = (await ThemeRepository.All().ConfigureAwait(false))
                .Select(theme => theme.Name)
                .Order()
                .ToList();

            var themesToDelete = allMyBricksThemes.Except(bricksetThemes).ToList();

            if (themesToDelete.Any())
            {
                MessageHub.Publish(new DeletingThemesStart { AffectedThemes = themesToDelete });

                foreach (var theme in themesToDelete)
                {
                    var allMyBricksSubthemes = (await SubthemeRepository.AllForTheme(theme).ConfigureAwait(false))
                        .Select(subtheme => subtheme.Name)
                        .Order()
                        .ToList();

                    MessageHub.Publish(new DeletingSubthemesStart { AffectedTheme = theme, AffectedSubthemes = allMyBricksSubthemes });

                    foreach (var subtheme in allMyBricksSubthemes)
                    {
                        var allMyBricksSets = (await SetRepository.AllForSubtheme(theme, subtheme).ConfigureAwait(false))
                            .Select(set => set.SetId)
                            .ToList();

                        await DeleteSets(allMyBricksSets);
                    }

                    await SubthemeRepository.DeleteMany(theme, allMyBricksSubthemes).ConfigureAwait(false);

                    MessageHub.Publish(new DeletingSubthemesEnd { AffectedTheme = theme, AffectedSubthemes = allMyBricksSubthemes });
                }

                await ThemeRepository.DeleteMany(themesToDelete).ConfigureAwait(false);

                MessageHub.Publish(new DeletingThemesEnd { AffectedThemes = themesToDelete });
            }

            MessageHub.Publish(new ThemeSanitizerEnd());
        }
    }
}
