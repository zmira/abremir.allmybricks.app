using abremir.AllMyBricks.Platform.Models;
using System.IO;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface IFile
    {
        Task WriteAllBytes(string path, byte[] bytes);
        bool Exists(string path);
        void Delete(string path);
        void DeleteFileIfExists(string path);
        FileStream OpenRead(string path);
        FileStream OpenWrite(string path);
        FileAttributes GetAttributes(string path);
        FileSize GetFileSize(string path);
    }
}
