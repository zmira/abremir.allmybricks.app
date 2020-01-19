using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Flurl.Http;
using System;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.AssetManagement.Services
{
    public class AssetManagementService : IAssetManagementService
    {
        private readonly IAssetUncompression _assetUncompression;
        private readonly IDirectory _directory;

        public AssetManagementService(
            IAssetUncompression assetUncompression,
            IDirectory directory)
        {
            _assetUncompression = assetUncompression;
            _directory = directory;
        }

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

            return _assetUncompression.UncompressAsset(await databaseSeedUrl.GetStreamAsync(), targetFolderPath ?? string.Empty);
        }
    }
}
