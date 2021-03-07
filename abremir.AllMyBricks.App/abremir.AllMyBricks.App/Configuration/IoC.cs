using LightInject;

namespace abremir.AllMyBricks.App.Configuration
{
    public static class IoC
    {
        public static ServiceContainer IoCContainer { get; private set; }

        public static IServiceRegistry Configure(string allMyBricksOnboardingUrl)
        {
            IoCContainer = new ServiceContainer();

            Platform.Configuration.IoC.Configure(IoCContainer);
            Data.Configuration.IoC.Configure(IoCContainer);
            ThirdParty.Brickset.Configuration.IoC.Configure(IoCContainer);
            Onboarding.Configuration.IoC.Configure(allMyBricksOnboardingUrl, IoCContainer);
            DataSynchronizer.Configuration.IoC.Configure(IoCContainer);

            Onboarding.Configuration.FlurlConfiguration.Configure();

            return IoCContainer;
        }
    }
}
