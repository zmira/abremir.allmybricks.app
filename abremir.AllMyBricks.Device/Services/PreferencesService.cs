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

        public bool SynchronizeSetExtendedData
        {
            get
            {
                return _preferences.Get(nameof(SynchronizeSetExtendedData), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(SynchronizeSetExtendedData), value, Constants.PreferencesSharedName);
            }
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

        public bool SynchronizeAdditionalImages {
            get
            {
                return _preferences.Get(nameof(SynchronizeAdditionalImages), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(SynchronizeAdditionalImages), value, Constants.PreferencesSharedName);
            }
        }

        public bool SynchronizeInstructions {
            get
            {
                return _preferences.Get(nameof(SynchronizeInstructions), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(SynchronizeInstructions), value, Constants.PreferencesSharedName);
            }
        }

        public bool SynchronizeTags {
            get
            {
                return _preferences.Get(nameof(SynchronizeTags), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(SynchronizeTags), value, Constants.PreferencesSharedName);
            }
        }

        public bool SynchronizePrices {
            get
            {
                return _preferences.Get(nameof(SynchronizePrices), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(SynchronizePrices), value, Constants.PreferencesSharedName);
            }
        }

        public bool SynchronizeReviews {
            get
            {
                return _preferences.Get(nameof(SynchronizeReviews), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(SynchronizeReviews), value, Constants.PreferencesSharedName);
            }
        }
    }
}