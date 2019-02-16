using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using Easy.MessageHub;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class SetSynchronizer : ISetSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly ISetRepository _setRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IThemeRepository _themeRepository;
        private readonly ISubthemeRepository _subthemeRepository;
        private readonly IPreferencesService _preferencesService;
        private readonly IThumbnailSynchronizer _thumbnailSynchronizer;
        private readonly IMessageHub _messageHub;

        public SetSynchronizer(
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository,
            IPreferencesService preferencesService,
            IThumbnailSynchronizer thumbnailSynchronizer,
            IMessageHub messageHub)
        {
            _bricksetApiService = bricksetApiService;
            _setRepository = setRepository;
            _referenceDataRepository = referenceDataRepository;
            _themeRepository = themeRepository;
            _subthemeRepository = subthemeRepository;
            _preferencesService = preferencesService;
            _thumbnailSynchronizer = thumbnailSynchronizer;
            _messageHub = messageHub;
        }

        public async Task Synchronize(string apiKey, Theme theme, Subtheme subtheme)
        {
            _messageHub.Publish(new SetSynchronizerStart { ForSubtheme = true });

            for (var year = subtheme.YearFrom; year <= subtheme.YearTo; year++)
            {
                try
                {
                    _messageHub.Publish(new AcquiringSets { Theme = theme.Name, Subtheme = subtheme.Name, Year = year });

                    var getSetsParameters = new ParameterSets
                    {
                        ApiKey = apiKey,
                        Theme = theme.Name,
                        Subtheme = subtheme.Name.Replace("{None}", ""),
                        Year = year.ToString()
                    };

                    var bricksetSets = (await _bricksetApiService.GetSets(getSetsParameters)).ToList();

                    _messageHub.Publish(new SetsAcquired { Theme = theme.Name, Subtheme = subtheme.Name, Count = bricksetSets.Count, Year = year });

                    foreach (var bricksetSet in bricksetSets)
                    {
                        await AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet, year);
                    }
                }
                catch(Exception ex)
                {
                    _messageHub.Publish(new SetSynchronizerException { Exception = ex });
                }
            }

            _messageHub.Publish(new SetSynchronizerEnd { ForSubtheme = true });
        }

        public async Task Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp)
        {
            _messageHub.Publish(new SetSynchronizerStart { ForSubtheme = false });

            try
            {
                var getRecentlyUpdatedSetsParameters = new ParameterMinutesAgo
                {
                    ApiKey = apiKey,
                    MinutesAgo = (int)(DateTimeOffset.Now - previousUpdateTimestamp).TotalMinutes
                };

                var recentlyUpdatedSets = (await _bricksetApiService.GetRecentlyUpdatedSets(getRecentlyUpdatedSetsParameters)).ToList();

                _messageHub.Publish(new SetsAcquired { Count = recentlyUpdatedSets.Count });

                foreach (var themeGroup in recentlyUpdatedSets.GroupBy(bricksetSet => bricksetSet.Theme))
                {
                    var theme = _themeRepository.Get(themeGroup.Key);

                    foreach (var subthemeGroup in themeGroup.GroupBy(themeSets => themeSets.Subtheme))
                    {
                        var subtheme = _subthemeRepository.Get(theme.Name, subthemeGroup.Key);

                        foreach (var bricksetSet in subthemeGroup)
                        {
                            await AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _messageHub.Publish(new SetSynchronizerException { Exception = ex });
            }

            _messageHub.Publish(new SetSynchronizerEnd { ForSubtheme = false });
        }

        private async Task AddOrUpdateSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet, short? year = null)
        {
            _messageHub.Publish(new SynchronizingSet { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant, Year = year });

            try
            {
                var getSetParameters = new ParameterUserHashSetId
                {
                    ApiKey = apiKey,
                    SetID = bricksetSet.SetId
                };

                bricksetSet = await _bricksetApiService.GetSet(getSetParameters);

                if (bricksetSet == null)
                {
                    return;
                }

                var set = await MapSet(apiKey, theme, subtheme, bricksetSet);

                _setRepository.AddOrUpdate(set);

                await _thumbnailSynchronizer.Synchronize(set, true);
            }
            catch(Exception ex)
            {
                _messageHub.Publish(new SynchronizingSetException { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant, Exception = ex });
            }

            _messageHub.Publish(new SynchronizedSet { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant });
        }

        private async Task<Set> MapSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet)
        {
            var set = new Set
            {
                SetId = bricksetSet.SetId,
                Number = bricksetSet.Number,
                Name = bricksetSet.Name,
                Description = bricksetSet.Description?.SanitizeBricksetString(),
                NumberVariant = (byte)bricksetSet.NumberVariant,
                Theme = theme,
                Subtheme = subtheme,
                Year = string.IsNullOrWhiteSpace(bricksetSet.Year) ? (short?)null : short.Parse(bricksetSet.Year),
                Pieces = string.IsNullOrWhiteSpace(bricksetSet.Pieces) ? (short?)null : short.Parse(bricksetSet.Pieces),
                Minifigs = string.IsNullOrWhiteSpace(bricksetSet.Minifigs) ? (short?)null : short.Parse(bricksetSet.Minifigs),
                BricksetUrl = bricksetSet.BricksetUrl?.SanitizeBricksetString(),
                Released = bricksetSet.Released,
                LastUpdated = bricksetSet.LastUpdated
            };

            set.OwnedByTotal = (short)bricksetSet.OwnedByTotal;
            set.WantedByTotal = (short)bricksetSet.WantedByTotal;
            set.Rating = (float)bricksetSet.Rating;
            set.Category = _referenceDataRepository.GetOrAdd<Category>(bricksetSet.Category);
            set.PackagingType = _referenceDataRepository.GetOrAdd<PackagingType>(bricksetSet.PackagingType);
            set.ThemeGroup = _referenceDataRepository.GetOrAdd<ThemeGroup>(bricksetSet.ThemeGroup);
            set.Ean = string.IsNullOrWhiteSpace(bricksetSet.Ean) ? null : bricksetSet.Ean;
            set.Upc = string.IsNullOrWhiteSpace(bricksetSet.Upc) ? null : bricksetSet.Upc;
            set.Availability = string.IsNullOrWhiteSpace(bricksetSet.Availability) ? null : bricksetSet.Availability;
            set.AgeMin = string.IsNullOrWhiteSpace(bricksetSet.AgeMin) ? (byte?)null : byte.Parse(bricksetSet.AgeMin);
            set.AgeMax = string.IsNullOrWhiteSpace(bricksetSet.AgeMax) ? (byte?)null : byte.Parse(bricksetSet.AgeMax);
            set.Height = string.IsNullOrWhiteSpace(bricksetSet.Height) ? (float?)null : float.Parse(bricksetSet.Height, NumberStyles.Any, CultureInfo.InvariantCulture);
            set.Width = string.IsNullOrWhiteSpace(bricksetSet.Width) ? (float?)null : float.Parse(bricksetSet.Width, NumberStyles.Any, CultureInfo.InvariantCulture);
            set.Depth = string.IsNullOrWhiteSpace(bricksetSet.Depth) ? (float?)null : float.Parse(bricksetSet.Depth, NumberStyles.Any, CultureInfo.InvariantCulture);
            set.Weight = string.IsNullOrWhiteSpace(bricksetSet.Weight) ? (float?)null : float.Parse(bricksetSet.Weight, NumberStyles.Any, CultureInfo.InvariantCulture);
            set.Notes = bricksetSet.Notes?.SanitizeBricksetString();

            SetTagList(set, bricksetSet.Tags);
            SetPriceList(set, bricksetSet);

            await SetImageList(apiKey, set, bricksetSet);
            await SetInstructionList(apiKey, set, bricksetSet);

            return set;
        }

        private void SetTagList(Set set, string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return;
            }

            foreach (var tag in tags
                .Split(',')
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag))
            {
                set.Tags.Add(_referenceDataRepository.GetOrAdd<Tag>(tag));
            }
        }

        private async Task SetImageList(string apiKey, Set set, Sets bricksetSet)
        {
            if (bricksetSet.Image)
            {
                set.Images.Add(new Image
                {
                    ImageUrl = bricksetSet.ImageUrl?.SanitizeBricksetString(),
                    LargeThumbnailUrl = bricksetSet.LargeThumbnailUrl?.SanitizeBricksetString(),
                    ThumbnailUrl = bricksetSet.ThumbnailUrl?.SanitizeBricksetString()
                });
            }

            if (bricksetSet.AdditionalImageCount == 0)
            {
                return;
            }

            var getAdditionalImagesParameters = new ParameterSetId
            {
                ApiKey = apiKey,
                SetID = set.SetId
            };

            set.Images = (await _bricksetApiService.GetAdditionalImages(getAdditionalImagesParameters))
                    .ToImageEnumerable()
                    .ToList();
        }

        private void SetPriceList(Set set, Sets bricksetSet)
        {
            if (!string.IsNullOrWhiteSpace(bricksetSet.CaRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.CA,
                    Value = float.Parse(bricksetSet.CaRetailPrice, NumberStyles.Any, CultureInfo.InvariantCulture)
                });
            }

            if (!string.IsNullOrWhiteSpace(bricksetSet.EuRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.EU,
                    Value = float.Parse(bricksetSet.EuRetailPrice, NumberStyles.Any, CultureInfo.InvariantCulture)
                });
            }

            if (!string.IsNullOrWhiteSpace(bricksetSet.UkRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.UK,
                    Value = float.Parse(bricksetSet.UkRetailPrice, NumberStyles.Any, CultureInfo.InvariantCulture)
                });
            }

            if (!string.IsNullOrWhiteSpace(bricksetSet.UsRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.US,
                    Value = float.Parse(bricksetSet.UsRetailPrice, NumberStyles.Any, CultureInfo.InvariantCulture)
                });
            }
        }

        private async Task SetInstructionList(string apiKey, Set set, Sets bricksetSet)
        {
            if (bricksetSet.InstructionsCount == 0)
            {
                return;
            }

            var getInstructionsParameters = new ParameterSetId
            {
                ApiKey = apiKey,
                SetID = set.SetId
            };

            set.Instructions = (await _bricksetApiService.GetInstructions(getInstructionsParameters))
                    .ToInstructionEnumerable()
                    .ToList();
        }
    }
}