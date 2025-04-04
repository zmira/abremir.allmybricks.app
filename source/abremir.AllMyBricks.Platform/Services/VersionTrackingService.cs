using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.ApplicationModel;

namespace abremir.AllMyBricks.Platform.Services
{
    public class VersionTrackingService(IVersionTracking versionTracking) : IVersionTrackingService
    {
        private readonly IVersionTracking _versionTracking = versionTracking;

        public bool IsFirstLaunch => _versionTracking.IsFirstLaunchEver;
    }
}
