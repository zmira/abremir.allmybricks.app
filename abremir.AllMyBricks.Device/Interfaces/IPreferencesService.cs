using abremir.AllMyBricks.Device.Enumerations;

namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IPreferencesService
    {
        bool SynchronizeSetExtendedData { get; set; }
        bool SynchronizeAdditionalImages { get; set; }
        bool SynchronizeInstructions { get; set; }
        bool SynchronizeTags { get; set; }
        bool SynchronizePrices { get; set; }
        bool SynchronizeReviews { get; set; }
        ThumbnailCachingStrategyEnum ThumbnailCachingStrategy { get; set; }
        bool ClearThumbnailCache { get; set; }
        AutomaticDataSynchronizationOverConnectionEnum AutomaticDataSynchronization { get; set; }
        bool AllowDataSynchronizationInBackground { get; set; }
    }
}