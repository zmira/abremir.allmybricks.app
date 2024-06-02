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
        IAssetExpansion assetExpansion) : IAssetManagementService
    {
        private readonly IFileSystemService _fileSystemService = fileSystemService;
        private readonly IAssetCompression _assetCompression = assetCompression;
        private readonly IFile _file = file;
        private readonly IRepositoryService _repositoryService = repositoryService;
        private readonly ILogger _logger = loggerFactory.CreateLogger<AssetManagementService>();
        private readonly IAssetExpansion _assetExpansion = assetExpansion;

        public void CompressDatabaseFile(bool encrypted)
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            _logger.LogInformation("Compressing {Encrypting}AllMyBricks Database {FileName} {FileSize}", encrypted ? "and Encrypting " : string.Empty, Path.GetFileName(dbFilePath), _file.GetFileSize(dbFilePath));

            _assetCompression.CompressAsset(dbFilePath, _fileSystemService.GetLocalPathToDataFolder(), encryptionKey: encrypted ? Settings.BricksetApiKey : null);

            GetCompressedDatabaseFilePathIfExists(out var compressedFilePath, encrypted);

            _logger.LogInformation("Compressed {Encrypting}AllMyBricks Database {FileName} {FileSize}", encrypted ? "and Encrypted " : string.Empty, Path.GetFileName(compressedFilePath), _file.GetFileSize(compressedFilePath));
        }

        public async Task CompactAllMyBricksDatabase()
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            var fileName = Path.GetFileName(dbFilePath);
            var fileSize = _file.GetFileSize(dbFilePath);

            _logger.LogInformation("Compacting AllMyBricks Database {FileName} {FileSize}", fileName, fileSize);

            var compacted = await _repositoryService.CompactRepository().ConfigureAwait(false);

            if (compacted > 0)
            {
                _file.DeleteFileIfExists($"{dbFilePath}.tmp_compaction_space");

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

            _logger.LogInformation("Expanding {Encrypted}AllMyBricks Database {FileName} {FileSize}", encryptedText, Path.GetFileName(compressedFilePath), _file.GetFileSize(compressedFilePath));

            _assetExpansion.ExpandAsset(compressedFilePath, _fileSystemService.GetLocalPathToDataFolder(), encryptionKey: encrypted ? Settings.BricksetApiKey : null);

            GetDatabaseFilePathIfExists(out var dbFilePath);

            _logger.LogInformation("Expanded {Encrypted}AllMyBricks Database {FileName} {FileSize}", encryptedText, Path.GetFileName(dbFilePath), _file.GetFileSize(dbFilePath));
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
            dbFilePath = _fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile);

            return _file.Exists(dbFilePath);
        }

        private bool GetCompressedDatabaseFilePathIfExists(out string compressedFilePath, bool encrypted)
        {
            compressedFilePath = Path.Combine(_fileSystemService.GetLocalPathToDataFolder(), AssetCompression.GetCompressedAssetFileName(Constants.AllMyBricksDbFile, encrypted));

            return _file.Exists(compressedFilePath);
        }
    }
}
