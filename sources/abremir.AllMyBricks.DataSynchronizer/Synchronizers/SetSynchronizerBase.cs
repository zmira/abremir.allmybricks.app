﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public abstract class SetSynchronizerBase
    {
        protected readonly IInsightsRepository InsightsRepository;
        protected readonly IOnboardingService OnboardingService;
        protected readonly IBricksetApiService BricksetApiService;
        protected readonly ISetRepository SetRepository;
        protected readonly IReferenceDataRepository ReferenceDataRepository;
        protected readonly IThemeRepository ThemeRepository;
        protected readonly ISubthemeRepository SubthemeRepository;
        protected readonly IThumbnailSynchronizer ThumbnailSynchronizer;
        protected readonly IMessageHub MessageHub;

        protected SetSynchronizerBase(
            IInsightsRepository insightsRepository,
            IOnboardingService onboardingService,
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository,
            IThumbnailSynchronizer thumbnailSynchronizer,
            IMessageHub messageHub)
        {
            InsightsRepository = insightsRepository;
            OnboardingService = onboardingService;
            BricksetApiService = bricksetApiService;
            SetRepository = setRepository;
            ReferenceDataRepository = referenceDataRepository;
            ThemeRepository = themeRepository;
            SubthemeRepository = subthemeRepository;
            ThumbnailSynchronizer = thumbnailSynchronizer;
            MessageHub = messageHub;
        }

        protected async Task AddOrUpdateSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet, short? year = null)
        {
            MessageHub.Publish(new SynchronizingSetStart { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant, Year = year });

            try
            {
                var set = await MapSet(apiKey, theme, subtheme, bricksetSet).ConfigureAwait(false);

                await SetRepository.AddOrUpdate(set).ConfigureAwait(false);

                await ThumbnailSynchronizer.Synchronize(set, true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageHub.Publish(new SynchronizingSetException { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant, Exception = ex });
            }

            MessageHub.Publish(new SynchronizingSetEnd { Theme = theme.Name, Subtheme = subtheme?.Name, Name = bricksetSet.Name, Number = bricksetSet.Number, NumberVariant = bricksetSet.NumberVariant });
        }

        private async Task<Set> MapSet(string apiKey, Theme theme, Subtheme subtheme, Sets bricksetSet)
        {
            var set = bricksetSet.ToSet();

            set.Theme = theme;
            set.Subtheme = subtheme;
            set.Category = await ReferenceDataRepository.GetOrAdd<Category>(bricksetSet.Category).ConfigureAwait(false);
            set.PackagingType = await ReferenceDataRepository.GetOrAdd<PackagingType>(bricksetSet.PackagingType).ConfigureAwait(false);
            set.ThemeGroup = await ReferenceDataRepository.GetOrAdd<ThemeGroup>(bricksetSet.ThemeGroup).ConfigureAwait(false);

            await SetTagList(set, bricksetSet.ExtendedData?.Tags).ConfigureAwait(false);
            SetPriceList(set, bricksetSet.LegoCom);

            await SetImageList(apiKey, set, bricksetSet).ConfigureAwait(false);
            await SetInstructionList(apiKey, set, bricksetSet).ConfigureAwait(false);

            return set;
        }

        private async Task SetTagList(Set set, IEnumerable<string> tags)
        {
            if (tags is null)
            {
                return;
            }

            foreach (var tag in tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag))
            {
                set.Tags.Add(await ReferenceDataRepository.GetOrAdd<Tag>(tag).ConfigureAwait(false));
            }
        }

        private async Task SetImageList(string apiKey, Set set, Sets bricksetSet)
        {
            if (bricksetSet.Image is not null)
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

            set.Images = (await BricksetApiService.GetAdditionalImages(getAdditionalImagesParameters).ConfigureAwait(false))
                    .ToImageEnumerable()
                    .ToList();
        }

        private static void SetPriceList(Set set, SetLegoCom legoCom)
        {
            if (legoCom is null)
            {
                return;
            }

            if (legoCom.CA?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegion.CA,
                    Value = legoCom.CA.RetailPrice.Value
                });
            }

            if (legoCom.DE?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegion.DE,
                    Value = legoCom.DE.RetailPrice.Value
                });
            }

            if (legoCom.UK?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegion.UK,
                    Value = legoCom.UK.RetailPrice.Value
                });
            }

            if (legoCom.US?.RetailPrice.HasValue == true)
            {
                set.Prices.Add(new Price
                {
                    Region = PriceRegion.US,
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

            set.Instructions = (await BricksetApiService.GetInstructions(getInstructionsParameters).ConfigureAwait(false))
                    .ToInstructionEnumerable()
                    .ToList();
        }

        protected async Task<IList<Sets>> GetAllSetsFor(string apiKey, GetSetsParameters getSetsParameters)
        {
            getSetsParameters.ApiKey = apiKey;
            getSetsParameters.PageSize ??= Constants.BricksetDefaultPageSizeParameter;
            getSetsParameters.ExtendedData = true;

            List<Sets> foundSets = [];
            var pageNumber = 1;

            List<Sets> currentPageResults;
            do
            {
                getSetsParameters.PageNumber = pageNumber;

                currentPageResults = (await BricksetApiService.GetSets(getSetsParameters).ConfigureAwait(false)).ToList();

                foundSets.AddRange(currentPageResults);

                pageNumber++;
            } while (currentPageResults.Count == getSetsParameters.PageSize);

            return foundSets;
        }
    }
}
