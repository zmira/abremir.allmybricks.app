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
        public async Task Synchronize(Set set, bool requestFromSynchronizer = false)
        {
            messageHub.Publish(new ThumbnailSynchronizerStart());

            if (preferencesService.ThumbnailCachingStrategy is ThumbnailCachingStrategy.NeverCache
                || (preferencesService.ThumbnailCachingStrategy is ThumbnailCachingStrategy.OnlyCacheDisplayedThumbnails
                    && requestFromSynchronizer)
                || set is null
                || string.IsNullOrWhiteSpace(set.Images.FirstOrDefault()?.ThumbnailUrl))
            {
                messageHub.Publish(new ThumbnailSynchronizerEnd());
                return;
            }

            try
            {
                byte[] thumbnail = null;

                try
                {
                    thumbnail = await set.Images[0].ThumbnailUrl.GetBytesAsync().ConfigureAwait(false);

                    messageHub.Publish(new ThumbnailAcquired { Thumbnail = set.Images[0].ThumbnailUrl });
                }
                catch { }

                if (thumbnail is null || thumbnail.Length is 0)
                {
                    messageHub.Publish(new ThumbnailSynchronizerEnd());
                    return;
                }

                messageHub.Publish(new SynchronizingThumbnailStart { Thumbnail = set.Images[0].ThumbnailUrl });

                await fileSystemService.SaveThumbnailToCache(set.Theme.Name, set.Subtheme.Name, set.NumberWithVariant, thumbnail).ConfigureAwait(false);

                messageHub.Publish(new SynchronizingThumbnailEnd { Thumbnail = set.Images[0].ThumbnailUrl });
            }
            catch (Exception ex)
            {
                messageHub.Publish(new ThumbnailSynchronizerException { Exception = ex });
            }

            messageHub.Publish(new ThumbnailSynchronizerEnd());
        }
    }
}
