using abremir.AllMyBricks.Platform.Enumerations;

namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface IPreferencesService
    {
        ThumbnailCachingStrategy ThumbnailCachingStrategy { get; set; }
        bool ClearThumbnailCache { get; set; }
        AutomaticDataSynchronizationOverConnection AutomaticDataSynchronization { get; set; }
        bool AllowDataSynchronizationInBackground { get; set; }
    }
}
