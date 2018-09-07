using abremir.AllMyBricks.Core.Models;
using JsonFlatFileDataStore;
using System;

namespace abremir.AllMyBricks.DatabaseFeeder
{
    public sealed class Settings
    {
        private static readonly Lazy<DataStore> _store = new Lazy<DataStore>(() => new DataStore("appsettings.json", false, reloadBeforeGetCollection: true));

        private static DataStore Store => _store.Value;

        public static string OnboardingUrl
        {
            get => Store.GetItem<string>(nameof(OnboardingUrl));
            set => Store.ReplaceItemAsync<string>(nameof(OnboardingUrl), value, true);
        }

        public static string BricksetApiKey
        {
            get => Store.GetItem<string>(nameof(BricksetApiKey));
            set => Store.ReplaceItemAsync<string>(nameof(BricksetApiKey), value, true);
        }

        public static Identification DeviceIdentification
        {
            get => Store.GetItem<Identification>(nameof(DeviceIdentification));
            set => Store.ReplaceItemAsync<Identification>(nameof(DeviceIdentification), value, true);
        }

        private Settings() { }
    }
}