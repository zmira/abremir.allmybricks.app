using abremir.AllMyBricks.DatabaseFeeder.Implementations;
using abremir.AllMyBricks.Device.Interfaces;
using SimpleInjector;

namespace abremir.AllMyBricks.DatabaseFeeder.Configuration
{
    public static class IoC
    {
        public static Container IoCContainer { get; private set; }

        public static void Configure(string allMyBricksOnboardingUrl)
        {
            IoCContainer = new Container();

            DataSynchronizer.Configuration.IoC.Configure(IoCContainer);
            ThirdParty.Brickset.Configuration.IoC.Configure(IoCContainer);
            Data.Configuration.IoC.Configure(IoCContainer);
            Onboarding.Configuration.IoC.Configure(allMyBricksOnboardingUrl, IoCContainer);

            IoCContainer.Register<IPreferencesService, PreferencesService>(Lifestyle.Transient);
            IoCContainer.Register<ISecureStorageService, SecureStorageService>(Lifestyle.Transient);
            IoCContainer.Register<IDeviceInformationService, DeviceInformationService>(Lifestyle.Transient);
            IoCContainer.Register<IFileSystemService, FileSystemService>(Lifestyle.Transient);
        }
    }
}