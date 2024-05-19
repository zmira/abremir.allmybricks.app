using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Networking;

namespace abremir.AllMyBricks.Platform.Services
{
    public class ConnectivityService(IConnectivity connectivity) : IConnectivityService
    {
        public bool IsInternetAccessible => connectivity.NetworkAccess is NetworkAccess.Internet;
    }
}
