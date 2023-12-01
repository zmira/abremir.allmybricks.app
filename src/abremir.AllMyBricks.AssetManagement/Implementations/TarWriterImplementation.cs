using System.IO;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using SharpCompress.Writers.Tar;

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
