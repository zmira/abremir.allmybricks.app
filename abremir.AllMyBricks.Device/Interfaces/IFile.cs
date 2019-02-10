using abremir.AllMyBricks.Device.Models;
using System.IO;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Device.Interfaces
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