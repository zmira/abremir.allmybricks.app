using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;
using SimpleInjector;

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
            UserManagement.Configuration.IoC.Configure(IoCContainer);

            IoCContainer.Register<IPreferencesService, PreferencesService>(Lifestyle.Transient);
            IoCContainer.Register<ISecureStorageService, SecureStorageService>(Lifestyle.Transient);
            IoCContainer.Register<IDeviceInformationService, DeviceInformationService>(Lifestyle.Transient);
            IoCContainer.Register<IAssetManagementService, AssetManagementService>(Lifestyle.Transient);

            IoCContainer.Register<IFileSystemService, FileSystemService>(Lifestyle.Singleton);
            IoCContainer.Register<IMessageHub, MessageHub>(Lifestyle.Singleton);
            IoCContainer.Register(() => Logging.Factory, Lifestyle.Singleton);
            IoCContainer.Register<AssetUncompressionLogger>(Lifestyle.Singleton);
            IoCContainer.Register<SetSynchronizationServiceLogger>(Lifestyle.Singleton);
            IoCContainer.Register<SetSynchronizerLogger>(Lifestyle.Singleton);
            IoCContainer.Register<SubthemeSynchronizerLogger>(Lifestyle.Singleton);
            IoCContainer.Register<ThemeSynchronizerLogger>(Lifestyle.Singleton);
            IoCContainer.Register<ThumbnailSynchronizerLogger>(Lifestyle.Singleton);
            IoCContainer.Register<UserSynchronizationServiceLogger>(Lifestyle.Singleton);
            IoCContainer.Register<UserSynchronizerLogger>(Lifestyle.Singleton);
            IoCContainer.Collection.Register<IDatabaseSeederLogger>(
                typeof(AssetUncompressionLogger),
                typeof(SetSynchronizationServiceLogger),
                typeof(SetSynchronizerLogger),
                typeof(SubthemeSynchronizerLogger),
                typeof(ThemeSynchronizerLogger),
                typeof(ThumbnailSynchronizerLogger),
                typeof(UserSynchronizationServiceLogger),
                typeof(UserSynchronizerLogger));

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
