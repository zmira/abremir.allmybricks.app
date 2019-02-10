using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using Microsoft.Extensions.Logging;
using System.IO;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class AssetManagementService : IAssetManagementService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IAssetCompression _assetCompression;
        private readonly IFile _file;
        private readonly IRepositoryService _repositoryService;
        private readonly ILogger _logger;
        private readonly IAssetUncompression _assedUncompression;

        public AssetManagementService(
            IFileSystemService fileSystemService,
            IAssetCompression assetCompression,
            IFile file,
            IRepositoryService repositoryService,
            ILoggerFactory loggerFactory,
            IAssetUncompression assedUncompression)
        {
            _fileSystemService = fileSystemService;
            _assetCompression = assetCompression;
            _file = file;
            _repositoryService = repositoryService;
            _logger = loggerFactory.CreateLogger<AssetManagementService>();
            _assedUncompression = assedUncompression;
        }

        public void CompressDatabaseFile()
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            _logger.LogInformation($"Compressing AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");

            _assetCompression.CompressAsset(dbFilePath, _fileSystemService.GetLocalPathToDataFolder());

            GetCompressedDatabaseFilePathIfExists(out var compressedFilePath);
            _logger.LogInformation($"Compressed AllMyBricks Database {Path.GetFileName(compressedFilePath)} {_file.GetFileSize(compressedFilePath)}");
        }

        public void CompactAllMyBricksDatabase()
        {
            if (!GetDatabaseFilePathIfExists(out var dbFilePath))
            {
                return;
            }

            _logger.LogInformation($"Compacting AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");

            var compacted = _repositoryService.CompactRepository();

            if (compacted)
            {
                _file.DeleteFileIfExists($"{dbFilePath}.tmp_compaction_space");

                _logger.LogInformation($"Compacted AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");
            }
            else
            {
                _logger.LogError($"Failed to Compact AllMyBricks Database {Path.GetFileName(dbFilePath)}");
            }
        }

        public void UncompressDatabaseFile()
        {
            if(!GetCompressedDatabaseFilePathIfExists(out var compressedFilePath))
            {
                return;
            }

            _logger.LogInformation($"Uncompressing AllMyBricks Database {Path.GetFileName(compressedFilePath)} {_file.GetFileSize(compressedFilePath)}");

            _assedUncompression.UncompressAsset(compressedFilePath, _fileSystemService.GetLocalPathToDataFolder(), true);

            GetDatabaseFilePathIfExists(out var dbFilePath);
            _logger.LogInformation($"Uncompressed AllMyBricks Database {Path.GetFileName(dbFilePath)} {_file.GetFileSize(dbFilePath)}");
        }

        public bool DatabaseFilePathExists()
        {
            return GetDatabaseFilePathIfExists(out _);
        }

        public bool CompressedDatabaseFilePathExists()
        {
            return GetCompressedDatabaseFilePathIfExists(out _);
        }

        private bool GetDatabaseFilePathIfExists(out string dbFilePath)
        {
            dbFilePath = _fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile);

            return _file.Exists(dbFilePath);
        }

        private bool GetCompressedDatabaseFilePathIfExists(out string compressedFilePath)
        {
            compressedFilePath = Path.Combine(_fileSystemService.GetLocalPathToDataFolder(), _assetCompression.GetCompressedAssetFileName(Constants.AllMyBricksDbFile));

            return _file.Exists(compressedFilePath);
        }
    }
}
