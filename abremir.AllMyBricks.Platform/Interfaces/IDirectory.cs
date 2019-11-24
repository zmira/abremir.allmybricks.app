using System.IO;

namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface IDirectory
    {
        bool Exists(string path);
        DirectoryInfo CreateDirectory(string path);
    }
}
