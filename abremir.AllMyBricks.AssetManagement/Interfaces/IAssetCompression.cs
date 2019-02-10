﻿using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IAssetCompression
    {
        bool CompressAsset(string originFilePath, string destinationFolderPath, bool overwrite = true);
        string GetCompressedAssetFileName(string uncompressedFilePath);
    }
}
