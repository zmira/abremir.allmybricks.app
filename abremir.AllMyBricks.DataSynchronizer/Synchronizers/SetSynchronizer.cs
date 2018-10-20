using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System;
using System.Globalization;
using System.Linq;

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
        private readonly IDataSynchronizerEventManager _dataSynchronizerEventHandler;

        public SetSynchronizer(
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository,
            IPreferencesService preferencesService,
            IThumbnailSynchronizer thumbnailSynchronizer,
            IDataSynchronizerEventManager dataSynchronizerEventHandler)
        {
            _bricksetApiService = bricksetApiService;
            _setRepository = setRepository;
            _referenceDataRepository = referenceDataRepository;
            _themeRepository = themeRepository;
            _subthemeRepository = subthemeRepository;
            _preferencesService = preferencesService;
            _thumbnailSynchronizer = thumbnailSynchronizer;
            _dataSynchronizerEventHandler = dataSynchronizerEventHandler;
        }

        public void Synchronize(string apiKey, Theme theme, Subtheme subtheme)
        {
            _dataSynchronizerEventHandler.Raise(new SetSynchronizerStart());

            for (var year = subtheme.YearFrom; year <= subtheme.YearTo; year++)
            {
                try
                {
                    var getSetsParameters = new ParameterSets
                    {
                        ApiKey = apiKey,
                        Theme = theme.Name,
                        Subtheme = subtheme.Name.Replace("{None}", ""),
                        Year = year.ToString()
                    };

                    var bricksetSets = _bricksetApiService.GetSets(getSetsParameters).ToList();

                    _dataSynchronizerEventHandler.Raise(new SetsAcquired { Theme = theme.Name, Subtheme = subtheme.Name, Count = bricksetSets.Count, Year = year });

                    foreach (var bricksetSet in bricksetSets)
                    {
                        AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet);
                    }
                }
                catch(Exception ex)
                {
                    _dataSynchronizerEventHandler.Raise(new SetSynchronizerException { Exception = ex });
                }
            }

            _dataSynchronizerEventHandler.Raise(new SetSynchronizerEnd());
        }

        public void Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp)
        {
            _dataSynchronizerEventHandler.Raise(new SetSynchronizerStart());

            try
            {
                var getRecentlyUpdatedSetsParameters = new ParameterMinutesAgo
                {
                    ApiKey = apiKey,
                    MinutesAgo = (int)(DateTimeOffset.Now - previousUpdateTimestamp).TotalMinutes
                };

                var recentlyUpdatedSets = _bricksetApiService.GetRecentlyUpdatedSets(getRecentlyUpdatedSetsParameters).ToList();

                _dataSynchronizerEventHandler.Raise(new SetsAcquired { Count = recentlyUpdatedSets.Count });

                foreach (var themeGroup in recentlyUpdatedSets.GroupBy(bricksetSet => bricksetSet.Theme))
                {
                    var theme = _themeRepository.Get(themeGroup.Key);

                    foreach (var subthemeGroup in themeGroup.GroupBy(themeSets => themeSets.Subtheme))
                    {
                        var subtheme = _subthemeRepository.Get(theme.Name, subthemeGroup.Key);

                        foreach (var bricksetSet in subthemeGroup)
                        {
                            AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _dataSynchronizerEventHandler.Raise(new SetSynchronizerException { Exception = ex });
            }

            _dataSynchronizerEventHandler.Raise(new SetSynchronizerEnd());
        }

        private Sets Synchronize(string apiKey, long setId)
        {
            var getSetParameters = new ParameterUserHashSetId
            {
                ApiKey = apiKey,
                SetID = setId
            };

            return _bricksetApiService.GetSet(getSetParameters);
        }

        private void AddOrUpdateSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet)
        {
            _dataSynchronizerEventHandler.Raise(new SynchronizingSet { Theme = theme.Name, Subtheme = subtheme.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant });

            try
            {
                if (_preferencesService.RetrieveFullSetDataOnSynchronization)
                {
                    bricksetSet = Synchronize(apiKey, bricksetSet.SetId);

                    if (bricksetSet == null)
                    {
                        return;
                    }
                }

                var set = MapSet(apiKey, theme, subtheme, bricksetSet);

                _setRepository.AddOrUpdate(set);

                _thumbnailSynchronizer.Synchronize(set, true);
            }
            catch(Exception ex)
            {
                _dataSynchronizerEventHandler.Raise(new SynchronizingSetException { Theme = theme.Name, Subtheme = subtheme.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant, Exception = ex });
            }

            _dataSynchronizerEventHandler.Raise(new SynchronizedSet { Theme = theme.Name, Subtheme = subtheme.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant });
        }

        private Set MapSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet)
        {
            var set = new Set
            {
                Theme = theme,
                Subtheme = subtheme,
                Category = _referenceDataRepository.GetOrAdd<Category>(bricksetSet.Category),
                PackagingType = _referenceDataRepository.GetOrAdd<PackagingType>(bricksetSet.PackagingType),
                ThemeGroup = _referenceDataRepository.GetOrAdd<ThemeGroup>(bricksetSet.ThemeGroup),
                SetId = bricksetSet.SetId,
                Number = bricksetSet.Number,
                Name = bricksetSet.Name,
                Description = bricksetSet.Description?.SanitizeBricksetString(),
                Ean = string.IsNullOrWhiteSpace(bricksetSet.Ean) ? null : bricksetSet.Ean,
                Upc = string.IsNullOrWhiteSpace(bricksetSet.Upc) ? null : bricksetSet.Upc,
                NumberVariant = (byte)bricksetSet.NumberVariant,
                Year = string.IsNullOrWhiteSpace(bricksetSet.Year) ? (short?)null : short.Parse(bricksetSet.Year),
                Pieces = string.IsNullOrWhiteSpace(bricksetSet.Pieces) ? (short?)null : short.Parse(bricksetSet.Pieces),
                Minifigs = string.IsNullOrWhiteSpace(bricksetSet.Minifigs) ? (short?)null : short.Parse(bricksetSet.Minifigs),
                BricksetUrl = bricksetSet.BricksetUrl?.SanitizeBricksetString(),
                Released = bricksetSet.Released,
                OwnedByTotal = (short)bricksetSet.OwnedByTotal,
                WantedByTotal = (short)bricksetSet.WantedByTotal,
                Rating = (float)bricksetSet.Rating,
                Availability = string.IsNullOrWhiteSpace(bricksetSet.Availability) ? null : bricksetSet.Availability,
                AgeMin = string.IsNullOrWhiteSpace(bricksetSet.AgeMin) ? (byte?)null : byte.Parse(bricksetSet.AgeMin),
                AgeMax = string.IsNullOrWhiteSpace(bricksetSet.AgeMax) ? (byte?)null : byte.Parse(bricksetSet.AgeMax),
                Height = string.IsNullOrWhiteSpace(bricksetSet.Height) ? (float?)null : float.Parse(bricksetSet.Height, NumberStyles.Any, CultureInfo.InvariantCulture),
                Width = string.IsNullOrWhiteSpace(bricksetSet.Width) ? (float?)null : float.Parse(bricksetSet.Width, NumberStyles.Any, CultureInfo.InvariantCulture),
                Depth = string.IsNullOrWhiteSpace(bricksetSet.Depth) ? (float?)null : float.Parse(bricksetSet.Depth, NumberStyles.Any, CultureInfo.InvariantCulture),
                Weight = string.IsNullOrWhiteSpace(bricksetSet.Weight) ? (float?)null : float.Parse(bricksetSet.Weight, NumberStyles.Any, CultureInfo.InvariantCulture),
                Notes = bricksetSet.Notes?.SanitizeBricksetString(),
                LastUpdated = bricksetSet.LastUpdated
            };

            SetTagList(set, bricksetSet.Tags);
            SetImageList(apiKey, set, bricksetSet);
            SetPriceList(set, bricksetSet);
            SetReviewList(apiKey, set, bricksetSet);
            SetInstructionList(apiKey, set, bricksetSet);

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

        private void SetImageList(string apiKey, Set set, Sets bricksetSet)
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

            if (bricksetSet.AdditionalImageCount > 0)
            {
                var getAdditionalImagesParameters = new ParameterSetId
                {
                    ApiKey = apiKey,
                    SetID = set.SetId
                };

                set.Images = _bricksetApiService
                        .GetAdditionalImages(getAdditionalImagesParameters)
                        .ToImageEnumerable()
                        .ToList();
            }
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

        private void SetReviewList(string apiKey, Set set, Sets bricksetSet)
        {
            if (bricksetSet.ReviewCount > 0)
            {
                var getReviewsParameters = new ParameterSetId
                {
                    ApiKey = apiKey,
                    SetID = set.SetId
                };

                set.Reviews = _bricksetApiService
                        .GetReviews(getReviewsParameters)
                        .ToReviewEnumerable()
                        .ToList();
            }
        }

        private void SetInstructionList(string apiKey, Set set, Sets bricksetSet)
        {
            if (bricksetSet.InstructionsCount > 0)
            {
                var getInstructionsParameters = new ParameterSetId
                {
                    ApiKey = apiKey,
                    SetID = set.SetId
                };

                set.Instructions = _bricksetApiService
                        .GetInstructions(getInstructionsParameters)
                        .ToInstructionEnumerable()
                        .ToList();
            }
        }
    }
}