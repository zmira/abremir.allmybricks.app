using abremir.AllMyBricks.Device.Enumerations;
using abremir.AllMyBricks.Device.Interfaces;
using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class PreferencesService : IPreferencesService
    {
        public bool SynchronizeSetExtendedData
        {
            get => Settings.SynchronizeSetExtendedData;
            set => Settings.SynchronizeSetExtendedData = value;
        }

        public ThumbnailCachingStrategyEnum ThumbnailCachingStrategy
        {
            get => ThumbnailCachingStrategyEnum.NeverCache;
            set => throw new NotImplementedException();
        }

        public bool ClearThumbnailCache
        {
            get => false;
            set => throw new NotImplementedException();
        }

        public AutomaticDataSynchronizationOverConnectionEnum AutomaticDataSynchronization
        {
            get => AutomaticDataSynchronizationOverConnectionEnum.Never;
            set => throw new NotImplementedException();
        }

        public bool AllowDataSynchronizationInBackground
        {
            get => true;
            set => throw new NotImplementedException();
        }

        public bool SynchronizeAdditionalImages {
            get => Settings.SynchronizeAdditionalImages;
            set => Settings.SynchronizeAdditionalImages = value;
        }

        public bool SynchronizeInstructions
        {
            get => Settings.SynchronizeInstructions;
            set => Settings.SynchronizeInstructions = value;
        }

        public bool SynchronizeTags
        {
            get => Settings.SynchronizeTags;
            set => Settings.SynchronizeTags = value;
        }

        public bool SynchronizePrices
        {
            get => Settings.SynchronizePrices;
            set => Settings.SynchronizePrices = value;
        }

        public bool SynchronizeReviews
        {
            get => Settings.SynchronizeReviews;
            set => Settings.SynchronizeReviews = value;
        }
    }
}