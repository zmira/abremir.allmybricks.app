using abremir.AllMyBricks.Platform.Enumerations;

namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface IPreferencesService
    {
        ThumbnailCachingStrategyEnum ThumbnailCachingStrategy { get; set; }
        bool ClearThumbnailCache { get; set; }
        AutomaticDataSynchronizationOverConnectionEnum AutomaticDataSynchronization { get; set; }
        bool AllowDataSynchronizationInBackground { get; set; }
    }
}
