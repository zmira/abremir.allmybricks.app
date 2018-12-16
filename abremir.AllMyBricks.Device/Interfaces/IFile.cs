using System.Threading.Tasks;

namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IFile
    {
        Task WriteAllBytes(string path, byte[] bytes);
    }
}