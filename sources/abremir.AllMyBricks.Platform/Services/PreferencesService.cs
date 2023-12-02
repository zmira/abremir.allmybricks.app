using abremir.AllMyBricks.Platform.Configuration;
using abremir.AllMyBricks.Platform.Enumerations;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Storage;

namespace abremir.AllMyBricks.Platform.Services
{
    public class PreferencesService : IPreferencesService
    {
        private readonly IPreferences _preferences;

        public PreferencesService(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public ThumbnailCachingStrategy ThumbnailCachingStrategy
        {
            get
            {
                return (ThumbnailCachingStrategy)_preferences.Get(nameof(ThumbnailCachingStrategy), (int)ThumbnailCachingStrategy.NeverCache, Constants.PreferencesSharedName);
            }
            set
            {
                ClearThumbnailCache |= (value == ThumbnailCachingStrategy.NeverCache && ThumbnailCachingStrategy != value);

                _preferences.Set(nameof(ThumbnailCachingStrategy), (int)value, Constants.PreferencesSharedName);
            }
        }

        public bool ClearThumbnailCache
        {
            get
            {
                return _preferences.Get(nameof(ClearThumbnailCache), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(ClearThumbnailCache), value, Constants.PreferencesSharedName);
            }
        }

        public AutomaticDataSynchronizationOverConnection AutomaticDataSynchronization
        {
            get
            {
                return (AutomaticDataSynchronizationOverConnection)_preferences.Get(nameof(AutomaticDataSynchronization), (int)AutomaticDataSynchronizationOverConnection.OnlyOverWiFiConnection, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(AutomaticDataSynchronization), (int)value, Constants.PreferencesSharedName);
            }
        }

        public bool AllowDataSynchronizationInBackground
        {
            get
            {
                return _preferences.Get(nameof(AllowDataSynchronizationInBackground), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(AllowDataSynchronizationInBackground), value, Constants.PreferencesSharedName);
            }
        }
    }
}
