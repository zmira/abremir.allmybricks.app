using abremir.AllMyBricks.Platform.Configuration;
using abremir.AllMyBricks.Platform.Enumerations;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Storage;

namespace abremir.AllMyBricks.Platform.Services
{
    public class PreferencesService(IPreferences preferences) : IPreferencesService
    {
        public ThumbnailCachingStrategy ThumbnailCachingStrategy
        {
            get
            {
                return (ThumbnailCachingStrategy)preferences.Get(nameof(ThumbnailCachingStrategy), (int)ThumbnailCachingStrategy.NeverCache, Constants.PreferencesSharedName);
            }
            set
            {
                ClearThumbnailCache |= (value is ThumbnailCachingStrategy.NeverCache && ThumbnailCachingStrategy != value);

                preferences.Set(nameof(ThumbnailCachingStrategy), (int)value, Constants.PreferencesSharedName);
            }
        }

        public bool ClearThumbnailCache
        {
            get
            {
                return preferences.Get(nameof(ClearThumbnailCache), false, Constants.PreferencesSharedName);
            }
            set
            {
                preferences.Set(nameof(ClearThumbnailCache), value, Constants.PreferencesSharedName);
            }
        }

        public AutomaticDataSynchronizationOverConnection AutomaticDataSynchronization
        {
            get
            {
                return (AutomaticDataSynchronizationOverConnection)preferences.Get(nameof(AutomaticDataSynchronization), (int)AutomaticDataSynchronizationOverConnection.OnlyOverWiFiConnection, Constants.PreferencesSharedName);
            }
            set
            {
                preferences.Set(nameof(AutomaticDataSynchronization), (int)value, Constants.PreferencesSharedName);
            }
        }

        public bool AllowDataSynchronizationInBackground
        {
            get
            {
                return preferences.Get(nameof(AllowDataSynchronizationInBackground), false, Constants.PreferencesSharedName);
            }
            set
            {
                preferences.Set(nameof(AllowDataSynchronizationInBackground), value, Constants.PreferencesSharedName);
            }
        }
    }
}
