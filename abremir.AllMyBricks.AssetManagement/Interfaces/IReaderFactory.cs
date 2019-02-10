using SharpCompress.Readers;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IReaderFactory
    {
        IReader Open(Stream stream);
    }
}
