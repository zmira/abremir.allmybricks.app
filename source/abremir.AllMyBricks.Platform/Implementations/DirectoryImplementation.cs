﻿using System.IO;
using abremir.AllMyBricks.Platform.Interfaces;

namespace abremir.AllMyBricks.Platform.Implementations
{
    public class DirectoryImplementation : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
