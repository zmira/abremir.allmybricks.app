using abremir.AllMyBricks.Device.Enumerations;
using abremir.AllMyBricks.Device.Interfaces;
using System;

namespace abremir.AllMyBricks.DatabaseFeeder.Implementations
{
    public class PreferencesService : IPreferencesService
    {
        public bool RetrieveFullSetDataOnSynchronization
        {
            get => true;
            set => throw new NotImplementedException();
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
    }
}