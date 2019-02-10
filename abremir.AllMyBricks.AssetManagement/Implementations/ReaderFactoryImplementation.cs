using abremir.AllMyBricks.AssetManagement.Interfaces;
using SharpCompress.Readers;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class ReaderFactoryImplementation : IReaderFactory
    {
        public IReader Open(Stream stream)
        {
            return ReaderFactory.Open(stream);
        }
    }
}
