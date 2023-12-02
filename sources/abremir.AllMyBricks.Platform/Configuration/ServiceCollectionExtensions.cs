using abremir.AllMyBricks.Platform.Implementations;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.Platform.Services;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace abremir.AllMyBricks.Platform.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformServices(this IServiceCollection services)
        {
            Guard.IsNotNull(services);

            return services
                .AddSingleton<IFileSystem>((_) => FileSystem.Current)
                .AddSingleton<IVersionTracking>((_) => VersionTracking.Default)
                .AddSingleton<IConnectivity>((_) => Connectivity.Current)
                .AddSingleton<ISecureStorage>((_) => SecureStorage.Default)
                .AddSingleton<IDeviceInfo>((_) => DeviceInfo.Current)
                .AddSingleton<IPreferences>((_) => Preferences.Default)
                .AddTransient<IFileSystemService, FileSystemService>()
                .AddTransient<IVersionTrackingService, VersionTrackingService>()
                .AddTransient<IConnectivityService, ConnectivityService>()
                .AddTransient<ISecureStorageService, SecureStorageService>()
                .AddTransient<IDeviceInformationService, DeviceInformationService>()
                .AddTransient<IPreferencesService, PreferencesService>()
                .AddPlatformIoServices();
        }

        public static IServiceCollection AddPlatformIoServices(this IServiceCollection services)
        {
            Guard.IsNotNull(services);

            return services
                .AddTransient<IFile, FileImplementation>()
                .AddTransient<IDirectory, DirectoryImplementation>();
        }
    }
}
