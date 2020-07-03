using abremir.AllMyBricks.Platform.Implementations;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.Platform.Services;
using LightInject;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Platform.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<IFileSystem, FileSystemImplementation>();
            container.Register<IVersionTracking, VersionTrackingImplementation>();
            container.Register<IConnectivity, ConnectivityImplementation>();
            container.Register<ISecureStorage, SecureStorageImplementation>();
            container.Register<IDeviceInfo, DeviceInfoImplementation>();
            container.Register<IPreferences, PreferencesImplementation>();

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
