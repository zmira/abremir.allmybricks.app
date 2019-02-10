using abremir.AllMyBricks.Device.Interfaces;
using System.IO;

namespace abremir.AllMyBricks.Device.Implementations
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
