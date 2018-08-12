using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Device.Services;
using SimpleInjector;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container = container ?? new Container();

            container.Register<IFileSystem, FileSystemImplementation>(Lifestyle.Singleton);
            container.Register<IVersionTracking, VersionTrackingImplementation>(Lifestyle.Singleton);

            container.Register<IFileSystemService, FileSystemService>(Lifestyle.Singleton);
            container.Register<IVersionTrackingService, VersionTrackingService>(Lifestyle.Singleton);

            return container;
        }
    }
}