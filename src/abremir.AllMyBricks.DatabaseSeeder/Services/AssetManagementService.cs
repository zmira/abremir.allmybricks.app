using System.IO;
using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class AssetManagementService : IAssetManagementService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IAssetCompression _assetCompression;
        private readonly IFile _file;
        private readonly IRepositoryService _repositoryService;
        private readonly ILogger _logger;
        private readonly IAssetUncompression _assetUncompression;

        public AssetManagementService(
            IFileSystemService fileSystemService,
            IAssetCompression assetCompression,
            IFile file,
            IRepositoryService repositoryService,
            ILoggerFactory loggerFactory,
            IAssetUncompression assetUncompression)
        {
            _fileSystemService = fileSystemService;
            _assetCompression = assetCompression;
            _file = file;
            _repositoryService = repositoryService;
            _logger = loggerFactory.CreateLogger<AssetManagementService>();
            _assetUncompression = assetUncompression;
        }

        public void CompressDatabaseFile(bool encrypted)
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            _logger.LogInformation($"Compressing {(encrypted ? "and Encrypting " : string.Empty)}AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");

            _assetCompression.CompressAsset(dbFilePath, _fileSystemService.GetLocalPathToDataFolder(), encryptionKey: encrypted ? Settings.BricksetApiKey : null);

            GetCompressedDatabaseFilePathIfExists(out var compressedFilePath, encrypted);
            _logger.LogInformation($"Compressed {(encrypted ? "and Encrypted " : string.Empty)}AllMyBricks Database {Path.GetFileName(compressedFilePath)} {_file.GetFileSize(compressedFilePath)}");
        }

        public void CompactAllMyBricksDatabase()
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            _logger.LogInformation($"Compacting AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");

            var compacted = _repositoryService.CompactRepository();

            if (compacted > 0)
            {
                _file.DeleteFileIfExists($"{dbFilePath}.tmp_compaction_space");

                _logger.LogInformation($"Compacted AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");
            }
            else
            {
                _logger.LogWarning($"Either Database did not need to be compacted or Failed to Compact AllMyBricks Database {Path.GetFileName(dbFilePath)}");
            }
        }

        public void UncompressDatabaseFile(bool encrypted)
        {
            if (!GetCompressedDatabaseFilePathIfExists(out var compressedFilePath, encrypted))
            {
                return;
            }

            _logger.LogInformation($"Uncompressing {(encrypted ? "Encrypted " : string.Empty)}AllMyBricks Database {Path.GetFileName(compressedFilePath)} {_file.GetFileSize(compressedFilePath)}");

            _assetUncompression.UncompressAsset(compressedFilePath, _fileSystemService.GetLocalPathToDataFolder(), encryptionKey: encrypted ? Settings.BricksetApiKey : null);

            GetDatabaseFilePathIfExists(out var dbFilePath);
            _logger.LogInformation($"Uncompressed {(encrypted ? "Encrypted " : string.Empty)}AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");
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
