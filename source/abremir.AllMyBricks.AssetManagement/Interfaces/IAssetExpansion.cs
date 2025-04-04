using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IAssetExpansion
    {
        bool ExpandAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true, string encryptionKey = null);
        bool ExpandAsset(Stream sourceStream, string targetFolderPath, bool overwrite = true, string encryptionKey = null);
    }
}
