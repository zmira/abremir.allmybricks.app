using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Flurl.Http;

namespace abremir.AllMyBricks.AssetManagement.Services
{
    public class AssetManagementService(
        IAssetExpansion assetExpansion,
        IDirectory directory,
        ISecureStorageService secureStorageService)
        : IAssetManagementService
    {
        private readonly IAssetExpansion _assetExpansion = assetExpansion;
        private readonly IDirectory _directory = directory;
        private readonly ISecureStorageService _secureStorageService = secureStorageService;

        public async Task<bool> InstallAllMyBricksSeedDatabase(string databaseSeedUrl, string targetFolderPath)
        {
            if ((!string.IsNullOrWhiteSpace(targetFolderPath)
                    && !_directory.Exists(targetFolderPath))
                || string.IsNullOrWhiteSpace(databaseSeedUrl)
                || !(Uri.TryCreate(databaseSeedUrl, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp)
                    && uriResult.Segments.Length > 1))
            {
                return false;
            }

            return _assetExpansion.ExpandAsset(await databaseSeedUrl.GetStreamAsync().ConfigureAwait(false), targetFolderPath ?? string.Empty, encryptionKey: await _secureStorageService.GetBricksetApiKey().ConfigureAwait(false));
        }
    }
}
