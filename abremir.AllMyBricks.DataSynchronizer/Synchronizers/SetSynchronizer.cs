using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System;
using System.Collections.Generic;
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

        public SetSynchronizer(
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository)
        {
            _bricksetApiService = bricksetApiService;
            _setRepository = setRepository;
            _referenceDataRepository = referenceDataRepository;
            _themeRepository = themeRepository;
            _subthemeRepository = subthemeRepository;
        }

        public IEnumerable<Set> Synchronize(string apiKey, Theme theme, Subtheme subtheme)
        {
            var processedSets = new List<Set>();

            for (var year = subtheme.YearFrom; year <= subtheme.YearTo; year++)
            {
                var getSetsParameters = new ParameterSets
                {
                    ApiKey = apiKey,
                    Theme = theme.Name,
                    Subtheme = subtheme.Name,
                    Year = year.ToString()
                };

                var setsRet = _bricksetApiService
                    .GetSets(getSetsParameters).ToList();

                foreach (var bricksetSet in setsRet)
                {
                    AddOrUpdateSet(processedSets, apiKey, theme, subtheme, bricksetSet);
                }
            }

            return processedSets;
        }

        public IEnumerable<Set> Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp)
        {
            var processedSets = new List<Set>();

            var getRecentlyUpdatedSetsParameters = new ParameterMinutesAgo
            {
                ApiKey = apiKey,
                MinutesAgo = (int)(DateTimeOffset.Now - previousUpdateTimestamp).TotalMinutes
            };

            foreach (var themeGroup in _bricksetApiService
                .GetRecentlyUpdatedSets(getRecentlyUpdatedSetsParameters)
                .GroupBy(bricksetSet => bricksetSet.Theme))
            {
                var theme = _themeRepository.Get(themeGroup.Key);

                foreach (var subthemeGroup in themeGroup
                    .GroupBy(themeSets => themeSets.Subtheme))
                {
                    var subtheme = _subthemeRepository.Get(theme.Name, subthemeGroup.Key);

                    foreach (var bricksetSet in subthemeGroup)
                    {
                        AddOrUpdateSet(processedSets, apiKey, theme, subtheme, bricksetSet);
                    }
                }
            }

            return processedSets;
        }

        public Set Synchronize(string apiKey, long setId)
        {
            var getSetParameters = new ParameterUserHashSetId
            {
                ApiKey = apiKey,
                SetID = setId
            };

            var bricksetSet = _bricksetApiService.GetSet(getSetParameters);

            if(bricksetSet == null)
            {
                return null;
            }

            var theme = _themeRepository.Get(bricksetSet.Theme);
            var subtheme = _subthemeRepository.Get(bricksetSet.Theme, bricksetSet.Subtheme);

            var set = MapSet(apiKey, theme, subtheme, bricksetSet);

            return _setRepository.AddOrUpdate(set);
        }

        private void AddOrUpdateSet(IList<Set> setList, string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet)
        {
            var set = MapSet(apiKey, theme, subtheme, bricksetSet);

            setList.Add(set);

            _setRepository.AddOrUpdate(set);
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