using System.IO;
using SharpCompress.Readers;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class ReaderFactoryImplementation : Interfaces.IReaderFactory
    {
        public IReader Open(Stream stream)
        {
            return ReaderFactory.Open(stream);
        }
    }
}
