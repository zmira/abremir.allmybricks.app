using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System;
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

        public bool Synchronize(string apiKey, Theme theme, Subtheme subtheme)
        {
            for (var year = subtheme.YearFrom; year <= subtheme.YearTo; year++)
            {
                var getSetsParameters = new ParameterSets
                {
                    ApiKey = apiKey,
                    Theme = theme.Name,
                    Subtheme = subtheme.Name,
                    Year = year.ToString()
                };

                foreach (var bricksetSet in _bricksetApiService
                    .GetSets(getSetsParameters))
                {
                    _setRepository.AddOrUpdate(MapSet(apiKey, theme, subtheme, bricksetSet));
                }
            }

            return true;
        }

        public bool Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp)
        {
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

                    foreach (var set in subthemeGroup)
                    {
                        _setRepository.AddOrUpdate(MapSet(apiKey, theme, subtheme, set));
                    }
                }
            }

            return true;
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
                Description = bricksetSet.Description,
                Ean = bricksetSet.Ean,
                Upc = bricksetSet.Upc,
                NumberVariant = (byte)bricksetSet.NumberVariant,
                Year = string.IsNullOrWhiteSpace(bricksetSet.Year) ? (short?)null : short.Parse(bricksetSet.Year),
                Pieces = string.IsNullOrWhiteSpace(bricksetSet.Pieces) ? (short?)null : short.Parse(bricksetSet.Pieces),
                Minifigs = string.IsNullOrWhiteSpace(bricksetSet.Minifigs) ? (short?)null : short.Parse(bricksetSet.Minifigs),
                BricksetUrl = bricksetSet.BricksetUrl,
                Released = bricksetSet.Released,
                OwnedByTotal = (short)bricksetSet.OwnedByTotal,
                WantedByTotal = (short)bricksetSet.WantedByTotal,
                Rating = (float)bricksetSet.Rating,
                Availability = bricksetSet.Availability,
                AgeMin = string.IsNullOrWhiteSpace(bricksetSet.AgeMin) ? (byte?)null : byte.Parse(bricksetSet.AgeMin),
                AgeMax = string.IsNullOrWhiteSpace(bricksetSet.AgeMax) ? (byte?)null : byte.Parse(bricksetSet.AgeMax),
                Height = string.IsNullOrWhiteSpace(bricksetSet.Height) ? (short?)null : byte.Parse(bricksetSet.Height),
                Width = string.IsNullOrWhiteSpace(bricksetSet.Width) ? (short?)null : byte.Parse(bricksetSet.Width),
                Depth = string.IsNullOrWhiteSpace(bricksetSet.Depth) ? (short?)null : byte.Parse(bricksetSet.Depth),
                Weight = string.IsNullOrWhiteSpace(bricksetSet.Weight) ? (short?)null : byte.Parse(bricksetSet.Weight),
                Notes = bricksetSet.Notes,
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
                    ImageUrl = bricksetSet.ImageUrl,
                    LargeThumbnailUrl = bricksetSet.LargeThumbnailUrl,
                    ThumbnailUrl = bricksetSet.ThumbnailUrl
                });
            }

            if (bricksetSet.AdditionalImageCount > 0)
            {
                var getAdditionalImagesParameters = new ParameterSetId
                {
                    ApiKey = apiKey,
                    SetID = set.SetId
                };

                foreach (var additionalImage in _bricksetApiService
                    .GetAdditionalImages(getAdditionalImagesParameters))
                {
                    set.Images.Add(new Image
                    {
                        ImageUrl = additionalImage.ImageUrl,
                        LargeThumbnailUrl = additionalImage.LargeThumbnailUrl,
                        ThumbnailUrl = additionalImage.ThumbnailUrl
                    });
                }
            }
        }

        private void SetPriceList(Set set, Sets bricksetSet)
        {
            if (!string.IsNullOrWhiteSpace(bricksetSet.CaRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.CA,
                    Value = float.Parse(bricksetSet.CaRetailPrice)
                });
            }

            if (!string.IsNullOrWhiteSpace(bricksetSet.EuRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.EU,
                    Value = float.Parse(bricksetSet.EuRetailPrice)
                });
            }

            if (!string.IsNullOrWhiteSpace(bricksetSet.UkRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.UK,
                    Value = float.Parse(bricksetSet.UkRetailPrice)
                });
            }

            if (!string.IsNullOrWhiteSpace(bricksetSet.UsRetailPrice))
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.US,
                    Value = float.Parse(bricksetSet.UsRetailPrice)
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

                foreach (var bricksetReview in _bricksetApiService
                    .GetReviews(getReviewsParameters))
                {
                    var review = new Review
                    {
                        Author = bricksetReview.Author,
                        DatePosted = bricksetReview.DatePosted,
                        ReviewContent = bricksetReview.Review,
                        Title = bricksetReview.Title,
                        Html = bricksetReview.Html
                    };

                    review.RatingComponents.Add(new RatingItem
                    {
                        Type = RatingItemEnum.Overall,
                        Value = (byte)bricksetReview.OverallRating
                    });

                    review.RatingComponents.Add(new RatingItem
                    {
                        Type = RatingItemEnum.Parts,
                        Value = (byte)bricksetReview.Parts
                    });

                    review.RatingComponents.Add(new RatingItem
                    {
                        Type = RatingItemEnum.BuildingExperience,
                        Value = (byte)bricksetReview.BuildingExperience
                    });

                    review.RatingComponents.Add(new RatingItem
                    {
                        Type = RatingItemEnum.Playability,
                        Value = (byte)bricksetReview.Playability
                    });

                    review.RatingComponents.Add(new RatingItem
                    {
                        Type = RatingItemEnum.ValueForMoney,
                        Value = (byte)bricksetReview.ValueForMoney
                    });

                    set.Reviews.Add(review);
                }
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

                foreach (var instruction in _bricksetApiService
                    .GetInstructions(getInstructionsParameters))
                {
                    set.Instructions.Add(new Instruction
                    {
                        Description = instruction.Description,
                        Url = instruction.Url
                    });
                }
            }
        }
    }
}