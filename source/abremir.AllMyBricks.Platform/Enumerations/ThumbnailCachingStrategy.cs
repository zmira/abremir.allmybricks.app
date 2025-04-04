using System.ComponentModel;

namespace abremir.AllMyBricks.Platform.Enumerations
{
    public enum ThumbnailCachingStrategy
    {
        [Description("Never cache thumbnails")]
        NeverCache,
        [Description("Only cache displayed thumbnails")]
        OnlyCacheDisplayedThumbnails,
        [Description("Cache all thumbnails when synchronizing")]
        CacheAllThumbnailsWhenSynchronizing
    }
}
