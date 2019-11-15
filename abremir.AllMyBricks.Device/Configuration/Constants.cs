namespace abremir.AllMyBricks.Device.Configuration
{
    public static class Constants
    {
        private const string AllMyBricks = "AllMyBricks";

        public static readonly string DeviceIdentificationSecureStorageKey = $"{AllMyBricks}.DeviceIdentification";
        public static readonly string BricksetApiKeySecureStorageKey = $"{AllMyBricks}.BricksetApiKey";
        public static readonly string BricksetPrimaryUsersStorageKey = $"{AllMyBricks}.BricksetPrimaryUsers";
        public static readonly string DefaultUsernameStorageKey = $"{AllMyBricks}.DefaultUsername";

        public const string AllMyBricksDataFolder = AllMyBricks;
        public const string ThumbnailCacheFolder = "Thumbnails";
        public const string FallbackFolderName = "-None-";

        public static readonly string PreferencesSharedName = $"{AllMyBricks}.Preferences";
    }
}
