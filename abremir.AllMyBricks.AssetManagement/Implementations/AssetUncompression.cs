using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class AssetUncompression : IAssetUncompression
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IReaderFactory _readerFactory;

        public AssetUncompression(
            IFile file,
            IDirectory directory,
            IReaderFactory readerFactory)
        {
            _file = file;
            _directory = directory;
            _readerFactory = readerFactory;
        }

        public bool UncompressAsset(string originFilePath, string destinationFolderPath, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(originFilePath)
                || !_file.Exists(originFilePath)
                || _file.GetAttributes(originFilePath).HasFlag(FileAttributes.Directory))
            {
                return false;
            }

            using (var originFileStream = _file.OpenRead(originFilePath))
            {
                return UncompressAsset(originFileStream, destinationFolderPath, overwrite);
            }
        }

        public bool UncompressAsset(Stream originStream, string destinationFolderPath, bool overwrite = true)
        {
            if (originStream == null
                || !_file.GetAttributes(destinationFolderPath).HasFlag(FileAttributes.Directory))
            {
                return false;
            }

            if (!_directory.Exists(destinationFolderPath))
            {
                _directory.CreateDirectory(destinationFolderPath);
            }

            originStream.Position = 0;

            using(var originReader = _readerFactory.Open(originStream))
            {
                while (originReader.MoveToNextEntry())
                {
                    if (!originReader.Entry.IsDirectory)
                    {
                        var destinationFilePath = Path.Combine(destinationFolderPath, originReader.Entry.Key);

                        if (overwrite)
                        {
                            _file.DeleteFileIfExists(destinationFilePath);
                        }

                        using (var destinationFileStream = _file.OpenWrite(destinationFilePath))
                        {
                            originReader.WriteEntryTo(destinationFileStream);
                        }
                    }
                }
            }

            return true;
        }
    }
}
