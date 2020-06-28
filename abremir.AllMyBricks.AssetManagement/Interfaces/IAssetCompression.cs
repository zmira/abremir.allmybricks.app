namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IAssetCompression
    {
        bool CompressAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true, string encryptionKey = null);
    }
}
