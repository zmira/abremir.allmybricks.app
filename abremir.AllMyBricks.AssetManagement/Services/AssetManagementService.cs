using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using Flurl.Http;
using System;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.AssetManagement.Services
{
    public class AssetManagementService : IAssetManagementService
    {
        private readonly IAssetUncompression _assetDecompression;
        private readonly IDirectory _directory;

        public AssetManagementService(
            IAssetUncompression assetDdecompression,
            IDirectory directory)
        {
            _assetDecompression = assetDdecompression;
            _directory = directory;
        }

        public async Task<bool> InstallAllMyBricksSeedDatabase(string databaseSeedUrl, string destinationFolderPath)
        {
            if((!string.IsNullOrWhiteSpace(destinationFolderPath)
                    && !_directory.Exists(destinationFolderPath))
                || string.IsNullOrWhiteSpace(databaseSeedUrl)
                || !(Uri.TryCreate(databaseSeedUrl, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp)
                    && uriResult.Segments.Length > 1))
            {
                return false;
            }

            return _assetDecompression.UncompressAsset(await databaseSeedUrl.GetStreamAsync(), destinationFolderPath ?? string.Empty);
        }
    }
}
