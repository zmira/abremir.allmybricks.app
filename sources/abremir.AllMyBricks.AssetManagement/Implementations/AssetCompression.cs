using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;

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

        public bool CompressAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true, string encryptionKey = null)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath)
                || !_file.Exists(sourceFilePath)
                || (!string.IsNullOrWhiteSpace(targetFolderPath)
                    && _directory.Exists(targetFolderPath)
                    && (_file.GetAttributes(targetFolderPath) & FileAttributes.Directory) == 0))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(targetFolderPath)
                && !_directory.Exists(targetFolderPath))
            {
                _directory.CreateDirectory(targetFolderPath);
            }

            var targetCompressedFilePath = Path.Combine(targetFolderPath ?? string.Empty, GetCompressedAssetFileName(sourceFilePath, false));
            var targetEncryptedFilePath = Path.Combine(targetFolderPath ?? string.Empty, GetCompressedAssetFileName(sourceFilePath, true));
            var encrypted = !string.IsNullOrWhiteSpace(encryptionKey);

            if (!overwrite
                && ((_file.Exists(targetCompressedFilePath) && !encrypted)
                    || (_file.Exists(targetEncryptedFilePath) && encrypted)))
            {
                return false;
            }

            _file.DeleteFileIfExists(targetCompressedFilePath);

            SaveCompressedFile(sourceFilePath, targetCompressedFilePath);
            EncryptCompressedFileIfRequired(encrypted, targetEncryptedFilePath, targetCompressedFilePath, encryptionKey);

            return true;
        }

        private void SaveCompressedFile(string sourceFilePath, string compressedFilePath)
        {
            using var sourceFileStream = _fileStream.CreateFileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var targetCompressedFileStream = _file.OpenWrite(compressedFilePath);

            var tarWriterOptions = new TarWriterOptions(CompressionType.LZip, true);

            using var targetWriter = _tarWriter.CreateTarWriter(targetCompressedFileStream, tarWriterOptions);

            targetWriter.Write(Path.GetFileName(sourceFilePath), sourceFileStream);
        }

        private void EncryptCompressedFileIfRequired(bool encrypted, string encryptedFilePath, string compressedFilePath, string encryptionKey)
        {
            if (encrypted)
            {
                _file.DeleteFileIfExists(encryptedFilePath);

                using var compressedFileStream = GetEncryptedStream(compressedFilePath, encryptionKey);
                using var targetEncryptedFileStream = _file.OpenWrite(encryptedFilePath);

                compressedFileStream.CopyTo(targetEncryptedFileStream);
                compressedFileStream.Flush();
                compressedFileStream.Close();
                targetEncryptedFileStream.Flush();
                targetEncryptedFileStream.Close();

                _file.DeleteFileIfExists(compressedFilePath);
            }
        }

        private Stream GetEncryptedStream(string sourceFilePath, string encryptionKey)
        {
            var inputStream = _fileStream.CreateFileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using var outputStream = new MemoryStream();

            var hash = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(encryptionKey));

            using var aes = Aes.Create();
            aes.Key = hash.Take(32).ToArray();
            aes.IV = hash.Take(16).ToArray();

            using var cryptoStreamEncryptor = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            var byteArrayInput = new byte[inputStream.Length];

            inputStream.Read(byteArrayInput, 0, byteArrayInput.Length);
            cryptoStreamEncryptor.Write(byteArrayInput, 0, byteArrayInput.Length);

            inputStream.Close();
            cryptoStreamEncryptor.FlushFinalBlock();

            outputStream.Flush();
            outputStream.Position = 0;

            return new MemoryStream(outputStream.ToArray());
        }

        public static string GetCompressedAssetFileName(string uncompressedFilePath, bool encrypted)
        {
            if (string.IsNullOrWhiteSpace(uncompressedFilePath))
            {
                return null;
            }

            return $"{Path.GetFileNameWithoutExtension(uncompressedFilePath)}.lz{(encrypted ? "c" : string.Empty)}";
        }
    }
}
