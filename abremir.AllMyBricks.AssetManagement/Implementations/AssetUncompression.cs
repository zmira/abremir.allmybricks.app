using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;
using SharpCompress.Common;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class AssetUncompression : IAssetUncompression
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IReaderFactory _readerFactory;
        private readonly IMessageHub _messageHub;

        public AssetUncompression(
            IFile file,
            IDirectory directory,
            IReaderFactory readerFactory,
            IMessageHub messageHub)
        {
            _file = file;
            _directory = directory;
            _readerFactory = readerFactory;
            _messageHub = messageHub;
        }

        public bool UncompressAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath)
                || !_file.Exists(sourceFilePath)
                || _file.GetAttributes(sourceFilePath).HasFlag(FileAttributes.Directory))
            {
                return false;
            }

            using var sourceFileStream = _file.OpenRead(sourceFilePath);

            return UncompressAsset(sourceFileStream, targetFolderPath, overwrite);
        }

        public bool UncompressAsset(Stream sourceStream, string targetFolderPath, bool overwrite = true)
        {
            if (sourceStream is null
                || (!string.IsNullOrWhiteSpace(targetFolderPath)
                    && _directory.Exists(targetFolderPath)
                    && !_file.GetAttributes(targetFolderPath).HasFlag(FileAttributes.Directory)))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(targetFolderPath)
                && !_directory.Exists(targetFolderPath))
            {
                _directory.CreateDirectory(targetFolderPath);
            }

            sourceStream.Position = 0;

            using var sourceReader = _readerFactory.Open(sourceStream);

            sourceReader.EntryExtractionProgress += SourceReader_EntryExtractionProgress;

            while (sourceReader.MoveToNextEntry())
            {
                if (!sourceReader.Entry.IsDirectory)
                {
                    var targetFilePath = Path.Combine(targetFolderPath ?? string.Empty, sourceReader.Entry.Key);

                    if (overwrite)
                    {
                        _file.DeleteFileIfExists(targetFilePath);
                    }

                    using var targetFileStream = _file.OpenWrite(targetFilePath);

                    sourceReader.WriteEntryTo(targetFileStream);
                }
            }

            return true;
        }

        private void SourceReader_EntryExtractionProgress(object sender, ReaderExtractionEventArgs<IEntry> entry)
        {
            _messageHub.Publish(entry);
        }
    }
}
