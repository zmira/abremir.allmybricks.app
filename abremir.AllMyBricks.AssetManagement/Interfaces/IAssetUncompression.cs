using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IAssetUncompression
    {
        bool UncompressAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true);
        bool UncompressAsset(Stream sourceStream, string targetFolderPath, bool overwrite = true);
    }
}
