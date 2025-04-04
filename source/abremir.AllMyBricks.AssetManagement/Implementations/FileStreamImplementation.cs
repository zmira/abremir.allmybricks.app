﻿using System.IO;
using abremir.AllMyBricks.AssetManagement.Interfaces;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class FileStreamImplementation : IFileStream
    {
        public FileStream CreateFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(path, mode, access, share);
        }
    }
}
