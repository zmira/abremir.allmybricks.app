using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;
using LightInject;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    public static class IoC
    {
        public static ServiceContainer IoCContainer { get; private set; }

        public static void Configure()
        {
            IoCContainer = new ServiceContainer();

            DataSynchronizer.Configuration.IoC.Configure(IoCContainer);
            ThirdParty.Brickset.Configuration.IoC.Configure(IoCContainer);
            Data.Configuration.IoC.Configure(IoCContainer);
            AssetManagement.Configuration.IoC.Configure(IoCContainer);
            UserManagement.Configuration.IoC.Configure(IoCContainer);
            Onboarding.Configuration.IoC.Configure(string.Empty, IoCContainer);

            IoCContainer.Register<IPreferencesService, PreferencesService>();
            IoCContainer.Register<ISecureStorageService, SecureStorageService>();
            IoCContainer.Register<IDeviceInformationService, DeviceInformationService>();
            IoCContainer.Register<IAssetManagementService, AssetManagementService>();

            IoCContainer.Register<IFileSystemService, FileSystemService>(new PerContainerLifetime());
            IoCContainer.Register<IMessageHub, MessageHub>(new PerContainerLifetime());
            IoCContainer.Register<AssetUncompressionLogger>(new PerContainerLifetime());
            IoCContainer.Register<SetSynchronizationServiceLogger>(new PerContainerLifetime());
            IoCContainer.Register<SetSynchronizerLogger>(new PerContainerLifetime());
            IoCContainer.Register<SubthemeSynchronizerLogger>(new PerContainerLifetime());
            IoCContainer.Register<ThemeSynchronizerLogger>(new PerContainerLifetime());
            IoCContainer.Register<ThumbnailSynchronizerLogger>(new PerContainerLifetime());
            IoCContainer.Register<UserSynchronizationServiceLogger>(new PerContainerLifetime());
            IoCContainer.Register<UserSynchronizerLogger>(new PerContainerLifetime());

            Onboarding.Configuration.FlurlConfiguration.Configure();
        }
    }
}
