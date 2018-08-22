using abremir.AllMyBricks.Device.Configuration;
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

        public bool RetrieveFullSetDataOnSynchronization
        {
            get
            {
                return _preferences.Get(nameof(RetrieveFullSetDataOnSynchronization), false, Constants.PreferencesSharedName);
            }
            set
            {
                _preferences.Set(nameof(RetrieveFullSetDataOnSynchronization), value, Constants.PreferencesSharedName);
            }
        }
    }
}