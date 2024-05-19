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
    public class AssetCompression(
        IFile file,
        IDirectory directory,
        IFileStream fileStream,
        ITarWriter tarWriter)
        : IAssetCompression
    {
        public bool CompressAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true, string encryptionKey = null)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath)
                || !file.Exists(sourceFilePath)
                || (!string.IsNullOrWhiteSpace(targetFolderPath)
                    && directory.Exists(targetFolderPath)
                    && (file.GetAttributes(targetFolderPath) & FileAttributes.Directory) is 0))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(targetFolderPath)
                && !directory.Exists(targetFolderPath))
            {
                directory.CreateDirectory(targetFolderPath);
            }

            var targetCompressedFilePath = Path.Combine(targetFolderPath ?? string.Empty, GetCompressedAssetFileName(sourceFilePath, false));
            var targetEncryptedFilePath = Path.Combine(targetFolderPath ?? string.Empty, GetCompressedAssetFileName(sourceFilePath, true));
            var encrypted = !string.IsNullOrWhiteSpace(encryptionKey);

            if (!overwrite
                && ((file.Exists(targetCompressedFilePath) && !encrypted)
                    || (file.Exists(targetEncryptedFilePath) && encrypted)))
            {
                return false;
            }

            file.DeleteFileIfExists(targetCompressedFilePath);

            SaveCompressedFile(sourceFilePath, targetCompressedFilePath);
            EncryptCompressedFileIfRequired(encrypted, targetEncryptedFilePath, targetCompressedFilePath, encryptionKey);

            return true;
        }

        private void SaveCompressedFile(string sourceFilePath, string compressedFilePath)
        {
            using var sourceFileStream = fileStream.CreateFileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var targetCompressedFileStream = file.OpenWrite(compressedFilePath);

            var tarWriterOptions = new TarWriterOptions(CompressionType.LZip, true);

            using var targetWriter = tarWriter.CreateTarWriter(targetCompressedFileStream, tarWriterOptions);

            targetWriter.Write(Path.GetFileName(sourceFilePath), sourceFileStream);
        }

        private void EncryptCompressedFileIfRequired(bool encrypted, string encryptedFilePath, string compressedFilePath, string encryptionKey)
        {
            if (encrypted)
            {
                file.DeleteFileIfExists(encryptedFilePath);

                using var compressedFileStream = GetEncryptedStream(compressedFilePath, encryptionKey);
                using var targetEncryptedFileStream = file.OpenWrite(encryptedFilePath);

                compressedFileStream.CopyTo(targetEncryptedFileStream);
                compressedFileStream.Flush();
                compressedFileStream.Close();
                targetEncryptedFileStream.Flush();
                targetEncryptedFileStream.Close();

                file.DeleteFileIfExists(compressedFilePath);
            }
        }

        private MemoryStream GetEncryptedStream(string sourceFilePath, string encryptionKey)
        {
            var inputStream = fileStream.CreateFileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using var outputStream = new MemoryStream();

            var hash = SHA256.HashData(Encoding.ASCII.GetBytes(encryptionKey));

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

        public static string GetCompressedAssetFileName(string expandedFilePath, bool encrypted)
        {
            if (string.IsNullOrWhiteSpace(expandedFilePath))
            {
                return null;
            }

            return $"{Path.GetFileNameWithoutExtension(expandedFilePath)}.lz{(encrypted ? "c" : string.Empty)}";
        }
    }
}
