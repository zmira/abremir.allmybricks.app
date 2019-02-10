using System.IO;

namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IDirectory
    {
        bool Exists(string path);
        DirectoryInfo CreateDirectory(string path);
    }
}
