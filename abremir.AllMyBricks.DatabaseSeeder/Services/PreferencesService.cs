using abremir.AllMyBricks.Platform.Enumerations;
using abremir.AllMyBricks.Platform.Interfaces;
using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class PreferencesService : IPreferencesService
    {
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
    }
}