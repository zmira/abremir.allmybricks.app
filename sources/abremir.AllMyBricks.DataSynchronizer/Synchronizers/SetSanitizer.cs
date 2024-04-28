using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSanitizer;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;
using LiteDB;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class SetSanitizer : SetSynchronizerBase, ISetSanitizer
    {
        public SetSanitizer(
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
            MessageHub.Publish(new SetSanitizerStart());

            var apiKey = await OnboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                MessageHub.Publish(new SetSanitizerException { Exception = exception });

                throw exception;
            }

            var (ExpectedNumberOfSets, ActualNumberOfSets) = await GetSetNumbers();

            if (ActualNumberOfSets == ExpectedNumberOfSets)
            {
                MessageHub.Publish(new SetSanitizerEnd());

                return;
            }

            MessageHub.Publish(new MismatchingNumberOfSetsWarning { Expected = ExpectedNumberOfSets, Actual = ActualNumberOfSets });

            var numberOfSetsPerYearFromSets = (await SetRepository.All().ConfigureAwait(false))
                .GroupBy(set => set.Year)
                .ToFrozenDictionary(group => group.Key, group => group.Count());
            var numberOfSetsPerYearFromThemes = (await ThemeRepository.All().ConfigureAwait(false))
                .SelectMany(theme => theme.SetCountPerYear)
                .GroupBy(setCountPerYear => setCountPerYear.Year)
                .ToFrozenDictionary(group => group.Key, group => group.Sum(value => value.SetCount));

            Dictionary<short, HashSet<string>> themesWithDifferences = [];

            var yearIterator = numberOfSetsPerYearFromSets.Keys.Length > numberOfSetsPerYearFromThemes.Keys.Length
                ? numberOfSetsPerYearFromSets
                : numberOfSetsPerYearFromThemes;
            var yearGetter = numberOfSetsPerYearFromSets.Keys.Length > numberOfSetsPerYearFromThemes.Keys.Length
                ? numberOfSetsPerYearFromThemes
                : numberOfSetsPerYearFromSets;

            foreach (var year in yearIterator.Keys.Order())
            {
                var fromThemeHasYear = yearGetter.TryGetValue(year, out var setCountForYearFromTheme);
                if (!fromThemeHasYear || yearIterator[year] != setCountForYearFromTheme)
                {
                    themesWithDifferences.TryAdd(year, []);

                    var setThemesFromYearWithDifference = (await SetRepository.AllForYear(year).ConfigureAwait(false))
                        .GroupBy(set => set.Theme.Name)
                        .ToFrozenDictionary(group => group.Key, group => group.Count());
                    var themesFromYearWithDifference = (await ThemeRepository.AllForYear(year).ConfigureAwait(false))
                        .Where(theme => theme.SetCountPerYear.Any(scpy => scpy.Year == year))
                        .ToFrozenDictionary(theme => theme.Name, theme => (int)theme.SetCountPerYear.First(setCountPerYear => setCountPerYear.Year == year).SetCount);

                    var themeIterator = setThemesFromYearWithDifference.Keys.Length > themesFromYearWithDifference.Keys.Length
                        ? setThemesFromYearWithDifference
                        : themesFromYearWithDifference;
                    var themeGetter = setThemesFromYearWithDifference.Keys.Length > themesFromYearWithDifference.Keys.Length
                        ? themesFromYearWithDifference
                        : setThemesFromYearWithDifference;

                    foreach (var theme in themeIterator.Keys.Order())
                    {
                        var fromThemesHasTheme = themeGetter.TryGetValue(theme, out var setCountForThemeFromTheme);
                        if (!fromThemesHasTheme || themeIterator[theme] != setCountForThemeFromTheme)
                        {
                            themesWithDifferences[year].Add(theme);
                        }
                    }
                }
            }

            if (themesWithDifferences.Count > 0)
            {
                MessageHub.Publish(new AdjustingThemesWithDifferencesStart { AffectedThemes = themesWithDifferences });

                foreach (var year in themesWithDifferences.Keys)
                {
                    var getSetsParameters = new GetSetsParameters
                    {
                        Theme = string.Join(",", themesWithDifferences[year]),
                        Year = year.ToString()
                    };

                    MessageHub.Publish(new AcquiringSetsStart { Type = SetAcquisitionType.Sanitize, Parameters = getSetsParameters });

                    var bricksetSetsFromThemeWithDifferences = await GetAllSetsFor(apiKey, getSetsParameters).ConfigureAwait(false);
                    var bricksetSetIds = bricksetSetsFromThemeWithDifferences
                        .Select(set => (long)set.SetId)
                        .Order();

                    MessageHub.Publish(new AcquiringSetsEnd { Count = bricksetSetsFromThemeWithDifferences.Count, Type = SetAcquisitionType.Sanitize, Parameters = getSetsParameters });

                    var identifiedThemes = themesWithDifferences[year].ToList();
                    var allMyBricksSetsFromThemeWithDifferences = (await SetRepository.Find(set => set.Year == year && identifiedThemes.Contains(set.Theme.Name)))
                        .Select(set => set.SetId)
                        .Order();

                    var setsToDelete = allMyBricksSetsFromThemeWithDifferences.Except(bricksetSetIds).ToList();

                    await DeleteSets(setsToDelete);

                    foreach (var bricksetSet in bricksetSetsFromThemeWithDifferences)
                    {
                        var theme = await ThemeRepository.Get(bricksetSet.Theme).ConfigureAwait(false);
                        var subtheme = await SubthemeRepository.Get(theme.Name, bricksetSet.Subtheme).ConfigureAwait(false);

                        await AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet).ConfigureAwait(false);
                    }
                }

                MessageHub.Publish(new AdjustingThemesWithDifferencesEnd { AffectedThemes = themesWithDifferences });
            }

            MessageHub.Publish(new SetSanitizerEnd());
        }
    }
}
