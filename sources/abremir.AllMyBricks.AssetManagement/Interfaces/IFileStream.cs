using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IFileStream
    {
        FileStream CreateFileStream(string path, FileMode mode, FileAccess access, FileShare share);
    }
}
