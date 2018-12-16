using abremir.AllMyBricks.Device.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Device.Implementations
{
    public class FileImplementation : IFile
    {
        public async Task WriteAllBytes(string path, byte[] bytes)
        {
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}