using abremir.AllMyBricks.Platform.Implementations;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.Platform.Services;
using SimpleInjector;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Platform.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container ??= new Container();

            container.Register<IFileSystem, FileSystemImplementation>(Lifestyle.Transient);
            container.Register<IVersionTracking, VersionTrackingImplementation>(Lifestyle.Transient);
            container.Register<IConnectivity, ConnectivityImplementation>(Lifestyle.Transient);
            container.Register<ISecureStorage, SecureStorageImplementation>(Lifestyle.Transient);
            container.Register<IDeviceInfo, DeviceInfoImplementation>(Lifestyle.Transient);
            container.Register<IPreferences, PreferencesImplementation>(Lifestyle.Transient);

            container.Register<IFileSystemService, FileSystemService>(Lifestyle.Transient);
            container.Register<IVersionTrackingService, VersionTrackingService>(Lifestyle.Transient);
            container.Register<IConnectivityService, ConnectivityService>(Lifestyle.Transient);
            container.Register<ISecureStorageService, SecureStorageService>(Lifestyle.Transient);
            container.Register<IDeviceInformationService, DeviceInformationService>(Lifestyle.Transient);
            container.Register<IPreferencesService, PreferencesService>(Lifestyle.Transient);

            return ConfigureIO(container);
        }

        public static Container ConfigureIO(Container container = null)
        {
            container ??= new Container();

            container.Register<IFile, FileImplementation>(Lifestyle.Transient);
            container.Register<IDirectory, DirectoryImplementation>(Lifestyle.Transient);

            return container;
        }
    }
}
