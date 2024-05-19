using System.IO;
using System.Threading.Tasks;
using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class AssetManagementService(
        IFileSystemService fileSystemService,
        IAssetCompression assetCompression,
        IFile file,
        IRepositoryService repositoryService,
        ILoggerFactory loggerFactory,
        IAssetExpansion assetExpansion)
        : IAssetManagementService
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<AssetManagementService>();

        public void CompressDatabaseFile(bool encrypted)
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            _logger.LogInformation("Compressing {Encrypting}AllMyBricks Database {FileName} {FileSize}", encrypted ? "and Encrypting " : string.Empty, Path.GetFileName(dbFilePath), file.GetFileSize(dbFilePath));

            assetCompression.CompressAsset(dbFilePath, fileSystemService.GetLocalPathToDataFolder(), encryptionKey: encrypted ? Settings.BricksetApiKey : null);

            GetCompressedDatabaseFilePathIfExists(out var compressedFilePath, encrypted);

            _logger.LogInformation("Compressed {Encrypting}AllMyBricks Database {FileName} {FileSize}", encrypted ? "and Encrypted " : string.Empty, Path.GetFileName(compressedFilePath), file.GetFileSize(compressedFilePath));
        }

        public async Task CompactAllMyBricksDatabase()
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            var fileName = Path.GetFileName(dbFilePath);
            var fileSize = file.GetFileSize(dbFilePath);

            _logger.LogInformation("Compacting AllMyBricks Database {FileName} {FileSize}", fileName, fileSize);

            var compacted = await repositoryService.CompactRepository().ConfigureAwait(false);

            if (compacted > 0)
            {
                file.DeleteFileIfExists($"{dbFilePath}.tmp_compaction_space");

                _logger.LogInformation("Compacted AllMyBricks Database {FileName} {FileSize}", fileName, fileSize);
            }
            else
            {
                _logger.LogWarning("Either Database did not need to be compacted or Failed to Compact AllMyBricks Database {FileName}", fileName);
            }
        }

        public void ExpandDatabaseFile(bool encrypted)
        {
            if (!GetCompressedDatabaseFilePathIfExists(out var compressedFilePath, encrypted))
            {
                return;
            }

            var encryptedText = encrypted ? "Encrypted " : string.Empty;

            _logger.LogInformation("Expanding {Encrypted}AllMyBricks Database {FileName} {FileSize}", encryptedText, Path.GetFileName(compressedFilePath), file.GetFileSize(compressedFilePath));

            assetExpansion.ExpandAsset(compressedFilePath, fileSystemService.GetLocalPathToDataFolder(), encryptionKey: encrypted ? Settings.BricksetApiKey : null);

            GetDatabaseFilePathIfExists(out var dbFilePath);

            _logger.LogInformation("Expanded {Encrypted}AllMyBricks Database {FileName} {FileSize}", encryptedText, Path.GetFileName(dbFilePath), file.GetFileSize(dbFilePath));
        }

        public bool DatabaseFilePathExists()
        {
            return GetDatabaseFilePathIfExists(out _);
        }

        public bool CompressedDatabaseFilePathExists(bool encrypted)
        {
            return GetCompressedDatabaseFilePathIfExists(out _, encrypted);
        }

        private bool GetDatabaseFilePathIfExists(out string dbFilePath)
        {
            dbFilePath = fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile);

            return file.Exists(dbFilePath);
        }

        private bool GetCompressedDatabaseFilePathIfExists(out string compressedFilePath, bool encrypted)
        {
            compressedFilePath = Path.Combine(fileSystemService.GetLocalPathToDataFolder(), AssetCompression.GetCompressedAssetFileName(Constants.AllMyBricksDbFile, encrypted));

            return file.Exists(compressedFilePath);
        }
    }
}
