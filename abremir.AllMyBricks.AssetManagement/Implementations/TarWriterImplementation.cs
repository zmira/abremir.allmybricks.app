using abremir.AllMyBricks.AssetManagement.Interfaces;
using SharpCompress.Writers.Tar;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class TarWriterImplementation : ITarWriter
    {
        public TarWriter CreateTarWriter(Stream destination, TarWriterOptions options)
        {
            return new TarWriter(destination, options);
        }
    }
}
