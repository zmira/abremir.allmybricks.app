using System;
using abremir.AllMyBricks.Platform.Enumerations;
using abremir.AllMyBricks.Platform.Interfaces;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class PreferencesService : IPreferencesService
    {
        public ThumbnailCachingStrategy ThumbnailCachingStrategy
        {
            get => ThumbnailCachingStrategy.NeverCache;
            set => throw new NotImplementedException();
        }

        public bool ClearThumbnailCache
        {
            get => false;
            set => throw new NotImplementedException();
        }

        public AutomaticDataSynchronizationOverConnection AutomaticDataSynchronization
        {
            get => AutomaticDataSynchronizationOverConnection.Never;
            set => throw new NotImplementedException();
        }

        public bool AllowDataSynchronizationInBackground
        {
            get => true;
            set => throw new NotImplementedException();
        }
    }
}
