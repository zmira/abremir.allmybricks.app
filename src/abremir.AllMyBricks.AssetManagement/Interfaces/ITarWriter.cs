using System.IO;
using SharpCompress.Writers.Tar;

namespace abremir.AllMyBricks.AssetManagement.Interfaces
{
    public interface ITarWriter
    {
        TarWriter CreateTarWriter(Stream destination, TarWriterOptions options);
    }
}
