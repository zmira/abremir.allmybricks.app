using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;
using System;
using System.Collections.Generic;
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
        private readonly IThumbnailSynchronizer _thumbnailSynchronizer;
        private readonly IMessageHub _messageHub;

        public SetSynchronizer(
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository,
            IThumbnailSynchronizer thumbnailSynchronizer,
            IMessageHub messageHub)
        {
            _bricksetApiService = bricksetApiService;
            _setRepository = setRepository;
            _referenceDataRepository = referenceDataRepository;
            _themeRepository = themeRepository;
            _subthemeRepository = subthemeRepository;
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
                    _messageHub.Publish(new AcquiringSetsStart { Theme = theme.Name, Subtheme = subtheme.Name, Year = year });

                    var getSetsParameters = new GetSetsParameters
                    {
                        ApiKey = apiKey,
                        Theme = theme.Name,
                        Subtheme = subtheme.Name.Replace("{None}", ""),
                        Year = year,
                        ExtendedData = true
                    };

                    var bricksetSets = (await _bricksetApiService.GetSets(getSetsParameters)).ToList();

                    _messageHub.Publish(new AcquiringSetsEnd { Theme = theme.Name, Subtheme = subtheme.Name, Count = bricksetSets.Count, Year = year });

                    foreach (var bricksetSet in bricksetSets)
                    {
                        await AddOrUpdateSet(apiKey, theme, subtheme, bricksetSet, year);
                    }
                }
                catch (Exception ex)
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
                var getSetsParameters = new GetSetsParameters
                {
                    ApiKey = apiKey,
                    UpdatedSince = previousUpdateTimestamp.UtcDateTime,
                    ExtendedData = true
                };

                var recentlyUpdatedSets = (await _bricksetApiService.GetSets(getSetsParameters)).ToList();

                _messageHub.Publish(new AcquiringSetsEnd { Count = recentlyUpdatedSets.Count });

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
            catch (Exception ex)
            {
                _messageHub.Publish(new SetSynchronizerException { Exception = ex });
            }

            _messageHub.Publish(new SetSynchronizerEnd { ForSubtheme = false });
        }

        private async Task AddOrUpdateSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet, short? year = null)
        {
            _messageHub.Publish(new SynchronizingSetStart { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant, Year = year });

            try
            {
                var set = await MapSet(apiKey, theme, subtheme, bricksetSet);

                _setRepository.AddOrUpdate(set);

                await _thumbnailSynchronizer.Synchronize(set, true);
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new SynchronizingSetException { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant, Exception = ex });
            }

            _messageHub.Publish(new SynchronizingSetEnd { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant });
        }

        private async Task<Set> MapSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet)
        {
            var set = bricksetSet.ToSet();

            set.Theme = theme;
            set.Subtheme = subtheme;
            set.Category = _referenceDataRepository.GetOrAdd<Category>(bricksetSet.Category);
            set.PackagingType = _referenceDataRepository.GetOrAdd<PackagingType>(bricksetSet.PackagingType);
            set.ThemeGroup = _referenceDataRepository.GetOrAdd<ThemeGroup>(bricksetSet.ThemeGroup);

            SetTagList(set, bricksetSet.ExtendedData?.Tags);
            SetPriceList(set, bricksetSet.LegoCom);

            await SetImageList(apiKey, set, bricksetSet);
            await SetInstructionList(apiKey, set, bricksetSet);

            return set;
        }

        private void SetTagList(Set set, IEnumerable<string> tags)
        {
            if (tags is null)
            {
                return;
            }

            foreach (var tag in tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag))
            {
                set.Tags.Add(_referenceDataRepository.GetOrAdd<Tag>(tag));
            }
        }

        private async Task SetImageList(string apiKey, Set set, Sets bricksetSet)
        {
            if (!(bricksetSet.Image is null))
            {
                set.Images.Add(new Image
                {
                    ImageUrl = bricksetSet.Image.ImageUrl?.SanitizeBricksetString(),
                    ThumbnailUrl = bricksetSet.Image.ThumbnailUrl?.SanitizeBricksetString()
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

        private void SetPriceList(Set set, SetLegoCom legoCom)
        {
            if (legoCom is null)
            {
                return;
            }

            if (legoCom.CA?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.CA,
                    Value = legoCom.CA.RetailPrice.Value
                });
            }

            if (legoCom.DE?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.DE,
                    Value = legoCom.DE.RetailPrice.Value
                });
            }

            if (legoCom.UK?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.UK,
                    Value = legoCom.UK.RetailPrice.Value
                });
            }

            if (legoCom.US?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegionEnum.US,
                    Value = legoCom.US.RetailPrice.Value
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
