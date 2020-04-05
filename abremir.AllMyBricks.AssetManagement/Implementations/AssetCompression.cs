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

        public bool CompressAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath)
                || !_file.Exists(sourceFilePath)
                || (!string.IsNullOrWhiteSpace(targetFolderPath)
                    && !_file.GetAttributes(targetFolderPath).HasFlag(FileAttributes.Directory)))
            {
                return false;
            }

            var targetFilePath = Path.Combine(targetFolderPath ?? string.Empty, GetCompressedAssetFileName(sourceFilePath));

            if (!overwrite && _file.Exists(targetFilePath))
            {
                return false;
            }

            _file.DeleteFileIfExists(targetFilePath);

            using var sourceFileStream = _fileStream.CreateFileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var targetFileStream = _file.OpenWrite(targetFilePath);

            var tarWriterOptions = new TarWriterOptions(CompressionType.LZip, true);

            using var targetWriter = _tarWriter.CreateTarWriter(targetFileStream, tarWriterOptions);

            targetWriter.Write(Path.GetFileName(sourceFilePath), sourceFileStream);

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
