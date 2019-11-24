using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class AssetCompression : IAssetCompression
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IFileStream _fileStream;
        private readonly ITarWriter _tarWriter;

        public AssetCompression(
            IFile file,
            IDirectory directory,
            IFileStream fileStream,
            ITarWriter tarWriter)
        {
            _file = file;
            _directory = directory;
            _fileStream = fileStream;
            _tarWriter = tarWriter;
        }

        public bool CompressAsset(string originFilePath, string destinationFolderPath, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(originFilePath)
                || !_file.Exists(originFilePath)
                || (!string.IsNullOrWhiteSpace(destinationFolderPath)
                    && !_file.GetAttributes(destinationFolderPath).HasFlag(FileAttributes.Directory)))
            {
                return false;
            }

            var destinationFilePath = Path.Combine(destinationFolderPath ?? string.Empty, GetCompressedAssetFileName(originFilePath));

            if(!overwrite && _file.Exists(destinationFilePath))
            {
                return false;
            }

            _file.DeleteFileIfExists(destinationFilePath);

            using (var originFileStream = _fileStream.CreateFileStream(originFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var destinationFileStream = _file.OpenWrite(destinationFilePath))
                {
                    var tarWriterOptions = new TarWriterOptions(CompressionType.LZip, true);

                    using (var destinationWriter = _tarWriter.CreateTarWriter(destinationFileStream, tarWriterOptions))
                    {
                        destinationWriter.Write(Path.GetFileName(originFilePath), originFileStream);
                    }
                }
            }

            return true;
        }

        public static string GetCompressedAssetFileName(string uncompressedFilePath)
        {
            if (string.IsNullOrWhiteSpace(uncompressedFilePath))
            {
                return null;
            }

            return $"{Path.GetFileNameWithoutExtension(uncompressedFilePath)}.lz";
        }
    }
}
