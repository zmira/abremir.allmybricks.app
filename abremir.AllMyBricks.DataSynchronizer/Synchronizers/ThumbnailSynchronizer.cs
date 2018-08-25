using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Enumerations;
using abremir.AllMyBricks.Device.Interfaces;
using Flurl.Http;
using System.Linq;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThumbnailSynchronizer : IThumbnailSynchronizer
    {
        private readonly IPreferencesService _preferencesService;
        private readonly IFileSystemService _fileSystemService;

        public ThumbnailSynchronizer(
            IPreferencesService preferencesService,
            IFileSystemService fileSystemService)
        {
            _preferencesService = preferencesService;
            _fileSystemService = fileSystemService;
        }

        public void Synchronize(Set set, bool requestFromSynchronizer = false)
        {
            if (_preferencesService.ThumbnailCachingStrategy == ThumbnailCachingStrategyEnum.NeverCache
                || (_preferencesService.ThumbnailCachingStrategy == ThumbnailCachingStrategyEnum.OnlyCacheDisplayedThumbnails
                    && requestFromSynchronizer)
                || set == null
                || string.IsNullOrWhiteSpace(set.Images.FirstOrDefault()?.ThumbnailUrl))
            {
                return;
            }

            byte[] thumbnail = null;

            try
            {
                thumbnail = set.Images[0].ThumbnailUrl.GetBytesAsync().Result;
            }
            catch { }

            if (thumbnail == null || thumbnail.Length == 0)
            {
                return;
            }

            _fileSystemService.SaveThumbnailToCache(set.Theme.Name, set.Subtheme.Name, set.NumberWithVariant, thumbnail);
        }
    }
}
