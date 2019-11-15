using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Enumerations;
using abremir.AllMyBricks.Device.Interfaces;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Services
{
    public class PreferencesService : IPreferencesService
    {
        private readonly IPreferences _preferences;

        public PreferencesService(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public ThumbnailCachingStrategyEnum ThumbnailCachingStrategy
        {
            get
            {
                return (ThumbnailCachingStrategyEnum)_preferences.Get(nameof(ThumbnailCachingStrategy), (int)ThumbnailCachingStrategyEnum.NeverCache, Constants.PreferencesSharedName);
            }
            set
            {
                ClearThumbnailCache |= (value == ThumbnailCachingStrategyEnum.NeverCache && ThumbnailCachingStrategy != value);

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

        public AutomaticDataSynchronizationOverConnectionEnum AutomaticDataSynchronization
        {
            get
            {
                return (AutomaticDataSynchronizationOverConnectionEnum)_preferences.Get(nameof(AutomaticDataSynchronization), (int)AutomaticDataSynchronizationOverConnectionEnum.OnlyOverWiFiConnection, Constants.PreferencesSharedName);
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
