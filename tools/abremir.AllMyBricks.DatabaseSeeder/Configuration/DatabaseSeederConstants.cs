namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    public static class DatabaseSeederConstants
    {
        public const string SyncCommand = "sync";
        public const string CompressCommand = "compress";
        public const string ExpandCommand = "expand";
        public const string CompactCommand = "compact";
        public const string SanitizeCommand = "sanitize";

        public const string LogVerbosityOption = "--log-verbosity";
        public const string LogDestinationOption = "--log-destination";
        public const string DatasetOption = "--dataset";
        public const string DataFolderOption = "--data-folder";
        public const string BricksetApiKeyOption = "--brickset-api-key";
        public const string EncryptedOption = "--encrypted";
    }
}
