using System;
using System.Collections.Generic;
using JsonFlatFileDataStore;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    public sealed class Settings
    {
        private static readonly Lazy<DataStore> _store = new(() => new DataStore("appsettings.json", false, reloadBeforeGetCollection: true));

        private static DataStore Store => _store.Value;

        public static string BricksetApiKey
        {
            get => Store.GetItem<string>(nameof(BricksetApiKey));
            set => Store.ReplaceItem<string>(nameof(BricksetApiKey), value, true);
        }

        public static Dictionary<string, string> BricksetPrimaryUsers
        {
            get => Store.GetItem<Dictionary<string, string>>(nameof(BricksetPrimaryUsers));
            set => Store.ReplaceItem<Dictionary<string, string>>(nameof(BricksetPrimaryUsers), value, true);
        }

        public static bool CompressedFileIsEncrypted
        {
            get => Store.GetItem<bool>(nameof(CompressedFileIsEncrypted));
            set => Store.ReplaceItem<bool>(nameof(CompressedFileIsEncrypted), value, true);
        }

        private Settings() { }
    }
}
