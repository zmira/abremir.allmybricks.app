using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Device.Enumerations;
using abremir.AllMyBricks.Device.Interfaces;
using Flurl.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThumbnailSynchronizer : IThumbnailSynchronizer
    {
        private readonly IPreferencesService _preferencesService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IDataSynchronizerEventManager _dataSynchronizerEventHandler;

        public ThumbnailSynchronizer(
            IPreferencesService preferencesService,
            IFileSystemService fileSystemService,
            IDataSynchronizerEventManager dataSynchronizerEventHandler)
        {
            _preferencesService = preferencesService;
            _fileSystemService = fileSystemService;
            _dataSynchronizerEventHandler = dataSynchronizerEventHandler;
        }

        public async Task Synchronize(Set set, bool requestFromSynchronizer = false)
        {
            _dataSynchronizerEventHandler.Raise(new ThumbnailSynchronizerStart());

            if (_preferencesService.ThumbnailCachingStrategy == ThumbnailCachingStrategyEnum.NeverCache
                || (_preferencesService.ThumbnailCachingStrategy == ThumbnailCachingStrategyEnum.OnlyCacheDisplayedThumbnails
                    && requestFromSynchronizer)
                || set == null
                || string.IsNullOrWhiteSpace(set.Images.FirstOrDefault()?.ThumbnailUrl))
            {
                _dataSynchronizerEventHandler.Raise(new ThumbnailSynchronizerEnd());
                return;
            }

            try
            {
                byte[] thumbnail = null;

                try
                {
                    thumbnail = set.Images[0].ThumbnailUrl.GetBytesAsync().Result;

                    _dataSynchronizerEventHandler.Raise(new ThumbnailAcquired { Thumbnail = set.Images[0].ThumbnailUrl });
                }
                catch { }

                if (thumbnail == null || thumbnail.Length == 0)
                {
                    _dataSynchronizerEventHandler.Raise(new ThumbnailSynchronizerEnd());
                    return;
                }

                _dataSynchronizerEventHandler.Raise(new SynchronizingThumbnail { Thumbnail = set.Images[0].ThumbnailUrl });

                await _fileSystemService.SaveThumbnailToCache(set.Theme.Name, set.Subtheme.Name, set.NumberWithVariant, thumbnail);

                _dataSynchronizerEventHandler.Raise(new SynchronizedThumbnail { Thumbnail = set.Images[0].ThumbnailUrl });
            }
            catch(Exception ex)
            {
                _dataSynchronizerEventHandler.Raise(new ThumbnailSynchronizerException { Exception = ex });
            }

            _dataSynchronizerEventHandler.Raise(new ThumbnailSynchronizerEnd());
        }
    }
}