using System;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Platform.Enumerations;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;
using Flurl.Http;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThumbnailSynchronizer(
        IPreferencesService preferencesService,
        IFileSystemService fileSystemService,
        IMessageHub messageHub)
        : IThumbnailSynchronizer
    {
        private readonly IPreferencesService _preferencesService = preferencesService;
        private readonly IFileSystemService _fileSystemService = fileSystemService;
        private readonly IMessageHub _messageHub = messageHub;

        public async Task Synchronize(Set set, bool requestFromSynchronizer = false)
        {
            _messageHub.Publish(new ThumbnailSynchronizerStart());

            if (_preferencesService.ThumbnailCachingStrategy is ThumbnailCachingStrategy.NeverCache
                || (_preferencesService.ThumbnailCachingStrategy is ThumbnailCachingStrategy.OnlyCacheDisplayedThumbnails
                    && requestFromSynchronizer)
                || set is null
                || string.IsNullOrWhiteSpace(set.Images.FirstOrDefault()?.ThumbnailUrl))
            {
                _messageHub.Publish(new ThumbnailSynchronizerEnd());
                return;
            }

            try
            {
                byte[] thumbnail = null;

                try
                {
                    thumbnail = await set.Images[0].ThumbnailUrl.GetBytesAsync().ConfigureAwait(false);

                    _messageHub.Publish(new ThumbnailAcquired { Thumbnail = set.Images[0].ThumbnailUrl });
                }
                catch { }

                if (thumbnail is null || thumbnail.Length is 0)
                {
                    _messageHub.Publish(new ThumbnailSynchronizerEnd());
                    return;
                }

                _messageHub.Publish(new SynchronizingThumbnailStart { Thumbnail = set.Images[0].ThumbnailUrl });

                await _fileSystemService.SaveThumbnailToCache(set.Theme.Name, set.Subtheme.Name, set.NumberWithVariant, thumbnail).ConfigureAwait(false);

                _messageHub.Publish(new SynchronizingThumbnailEnd { Thumbnail = set.Images[0].ThumbnailUrl });
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new ThumbnailSynchronizerException { Exception = ex });
            }

            _messageHub.Publish(new ThumbnailSynchronizerEnd());
        }
    }
}
