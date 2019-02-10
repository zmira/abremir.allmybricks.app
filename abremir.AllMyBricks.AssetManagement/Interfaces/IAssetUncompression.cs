using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IAssetUncompression
    {
        bool UncompressAsset(string originFilePath, string destinationFolderPath, bool overwrite = true);
        bool UncompressAsset(Stream originStream, string destinationFolderPath, bool overwrite = true);
    }
}
