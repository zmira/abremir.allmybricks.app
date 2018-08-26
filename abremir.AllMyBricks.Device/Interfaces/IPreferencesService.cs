using abremir.AllMyBricks.Device.Enumerations;

namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IPreferencesService
    {
        bool RetrieveFullSetDataOnSynchronization { get; set; }
        ThumbnailCachingStrategyEnum ThumbnailCachingStrategy { get; set; }
        bool ClearThumbnailCache { get; set; }
        AutomaticDataSynchronizationOverConnectionEnum AutomaticDataSynchronization { get; set; }
        bool AllowDataSynchronizationInBackground { get; set; }
    }
}