using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.Device.Interfaces;
using Easy.MessageHub;
using SimpleInjector;
using System.Reflection;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    public static class IoC
    {
        public static Container IoCContainer { get; private set; }

        public static void Configure()
        {
            IoCContainer = new Container();

            DataSynchronizer.Configuration.IoC.Configure(IoCContainer);
            ThirdParty.Brickset.Configuration.IoC.Configure(IoCContainer);
            Data.Configuration.IoC.Configure(IoCContainer);
            AssetManagement.Configuration.IoC.Configure(IoCContainer);

            IoCContainer.Register<IPreferencesService, PreferencesService>(Lifestyle.Transient);
            IoCContainer.Register<ISecureStorageService, SecureStorageService>(Lifestyle.Transient);
            IoCContainer.Register<IDeviceInformationService, DeviceInformationService>(Lifestyle.Transient);
            IoCContainer.Register<IFileSystemService, FileSystemService>(Lifestyle.Transient);
            IoCContainer.Register<IAssetManagementService, AssetManagementService>(Lifestyle.Transient);

            IoCContainer.Register<IMessageHub, MessageHub>(Lifestyle.Singleton);

            IoCContainer.Register(() => Logging.LoggerFactory, Lifestyle.Singleton);
            IoCContainer.Collection.Register<IDatabaseSeederLogger>(Assembly.GetExecutingAssembly());

            Onboarding.Configuration.FlurlConfiguration.Configure();
        }

        public static void ConfigureOnboarding(string allMyBricksOnboardingUrl)
        {
            Onboarding.Configuration.IoC.Configure(allMyBricksOnboardingUrl, IoCContainer);
        }

        public static void ReplaceOnboarding(string allMyBricksOnboardingUrl)
        {
            IoCContainer.Options.AllowOverridingRegistrations = true;

            ConfigureOnboarding(allMyBricksOnboardingUrl);

            IoCContainer.Options.AllowOverridingRegistrations = false;
        }
    }
}