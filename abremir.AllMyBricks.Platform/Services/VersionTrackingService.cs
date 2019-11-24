using abremir.AllMyBricks.Platform.Interfaces;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Platform.Services
{
    public class VersionTrackingService : IVersionTrackingService
    {
        private readonly IVersionTracking _versionTracking;

        public VersionTrackingService(IVersionTracking versionTracking)
        {
            _versionTracking = versionTracking;
        }

        public bool IsFirstLaunch => _versionTracking.IsFirstLaunchEver;
    }
}
