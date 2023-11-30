using System.IO;
using SharpCompress.Readers;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface IReaderFactory
    {
        IReader Open(Stream stream);
    }
}
