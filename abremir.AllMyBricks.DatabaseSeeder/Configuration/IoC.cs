using abremir.AllMyBricks.DatabaseSeeder.Implementations;
using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using abremir.AllMyBricks.Device.Interfaces;
using Microsoft.Extensions.Logging;
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

            IoCContainer.Register<IPreferencesService, PreferencesService>(Lifestyle.Transient);
            IoCContainer.Register<ISecureStorageService, SecureStorageService>(Lifestyle.Transient);
            IoCContainer.Register<IDeviceInformationService, DeviceInformationService>(Lifestyle.Transient);
            IoCContainer.Register<IFileSystemService, FileSystemService>(Lifestyle.Transient);
            IoCContainer.Register<ILoggerFactory>(() => Logging.LoggerFactory, Lifestyle.Singleton);
            IoCContainer.Register<DataSynchronizationServiceLogger>(Lifestyle.Transient);
            IoCContainer.Register<ThemeSynchronizerLogger>(Lifestyle.Transient);
            IoCContainer.Register<SubthemeSynchronizerLogger>(Lifestyle.Transient);
            IoCContainer.Register<SetSynchronizerLogger>(Lifestyle.Transient);
            IoCContainer.Register<ThumbnailSynchronizerLogger>(Lifestyle.Transient);

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