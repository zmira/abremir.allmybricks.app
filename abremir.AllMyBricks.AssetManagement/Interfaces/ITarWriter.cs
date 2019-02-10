using SharpCompress.Writers.Tar;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface ITarWriter
    {
        TarWriter CreateTarWriter(Stream destination, TarWriterOptions options);
    }
}
