using abremir.AllMyBricks.Device.Interfaces;
using System.IO;

namespace abremir.AllMyBricks.Device.Implementations
{
    public class FileImplementation : IFile
    {
        public void WriteAllBytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }
    }
}