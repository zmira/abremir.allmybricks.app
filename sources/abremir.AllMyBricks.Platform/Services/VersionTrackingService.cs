using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.ApplicationModel;

namespace abremir.AllMyBricks.Platform.Services
{
    public class VersionTrackingService(IVersionTracking versionTracking) : IVersionTrackingService
    {
        public bool IsFirstLaunch => versionTracking.IsFirstLaunchEver;
    }
}
