using abremir.AllMyBricks.Device.Interfaces;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Services
{
    public class ConnectivityService : IConnectivityService
    {
        private readonly IConnectivity _connectivity;

        public ConnectivityService(IConnectivity connectivity)
        {
            _connectivity = connectivity;
        }

        public bool IsInternetAccessible => _connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;
    }
}