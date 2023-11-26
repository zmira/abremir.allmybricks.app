using abremir.AllMyBricks.Platform.Implementations;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.Platform.Services;
using LightInject;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace abremir.AllMyBricks.Platform.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.RegisterSingleton<IFileSystem>((_) => FileSystem.Current);
            container.RegisterSingleton<IVersionTracking>((_) => VersionTracking.Default);
            container.RegisterSingleton<IConnectivity>((_) => Connectivity.Current);
            container.RegisterSingleton<ISecureStorage>((_) => SecureStorage.Default);
            container.RegisterSingleton<IDeviceInfo>((_) => DeviceInfo.Current);
            container.RegisterSingleton<IPreferences>((_) => Preferences.Default);

            container.Register<IFileSystemService, FileSystemService>();
            container.Register<IVersionTrackingService, VersionTrackingService>();
            container.Register<IConnectivityService, ConnectivityService>();
            container.Register<ISecureStorageService, SecureStorageService>();
            container.Register<IDeviceInformationService, DeviceInformationService>();
            container.Register<IPreferencesService, PreferencesService>();

            return ConfigureIO(container);
        }

        public static IServiceRegistry ConfigureIO(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<IFile, FileImplementation>();
            container.Register<IDirectory, DirectoryImplementation>();

            return container;
        }
    }
}
