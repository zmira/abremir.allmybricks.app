using abremir.AllMyBricks.Core.Models;
using JsonFlatFileDataStore;
using System;

namespace abremir.AllMyBricks.DatabaseSeeder
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

        public static bool SynchronizeSetExtendedData
        {
            get => Store.GetItem<bool>(nameof(SynchronizeSetExtendedData));
            set => Store.ReplaceItemAsync<bool>(nameof(SynchronizeSetExtendedData), value, true);
        }

        public static bool SynchronizeAdditionalImages
        {
            get => Store.GetItem<bool>(nameof(SynchronizeAdditionalImages));
            set => Store.ReplaceItemAsync<bool>(nameof(SynchronizeAdditionalImages), value, true);
        }

        public static bool SynchronizeInstructions
        {
            get => Store.GetItem<bool>(nameof(SynchronizeInstructions));
            set => Store.ReplaceItemAsync<bool>(nameof(SynchronizeInstructions), value, true);
        }

        public static bool SynchronizeTags
        {
            get => Store.GetItem<bool>(nameof(SynchronizeTags));
            set => Store.ReplaceItemAsync<bool>(nameof(SynchronizeTags), value, true);
        }

        public static bool SynchronizePrices
        {
            get => Store.GetItem<bool>(nameof(SynchronizePrices));
            set => Store.ReplaceItemAsync<bool>(nameof(SynchronizePrices), value, true);
        }

        public static bool SynchronizeReviews
        {
            get => Store.GetItem<bool>(nameof(SynchronizeReviews));
            set => Store.ReplaceItemAsync<bool>(nameof(SynchronizeReviews), value, true);
        }

        private Settings() { }
    }
}