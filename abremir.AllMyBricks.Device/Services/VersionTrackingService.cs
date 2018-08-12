using abremir.AllMyBricks.Device.Interfaces;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Services
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
