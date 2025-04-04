using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Networking;

namespace abremir.AllMyBricks.Platform.Services
{
    public class ConnectivityService(IConnectivity connectivity) : IConnectivityService
    {
        private readonly IConnectivity _connectivity = connectivity;

        public bool IsInternetAccessible => _connectivity.NetworkAccess is NetworkAccess.Internet;
    }
}
