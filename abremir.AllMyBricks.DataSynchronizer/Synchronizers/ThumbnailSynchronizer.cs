using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Platform.Enumerations;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;
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
        private readonly IMessageHub _messageHub;

        public ThumbnailSynchronizer(
            IPreferencesService preferencesService,
            IFileSystemService fileSystemService,
            IMessageHub messageHub)
        {
            _preferencesService = preferencesService;
            _fileSystemService = fileSystemService;
            _messageHub = messageHub;
        }

        public async Task Synchronize(Set set, bool requestFromSynchronizer = false)
        {
            _messageHub.Publish(new ThumbnailSynchronizerStart());

            if (_preferencesService.ThumbnailCachingStrategy == ThumbnailCachingStrategy.NeverCache
                || (_preferencesService.ThumbnailCachingStrategy == ThumbnailCachingStrategy.OnlyCacheDisplayedThumbnails
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
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch { }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body

                if (thumbnail is null || thumbnail.Length == 0)
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
