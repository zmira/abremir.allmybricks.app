using SimpleInjector;

namespace abremir.AllMyBricks.Core.Configuration
{
    public static class IoC
    {
        public static Container IoCContainer { get; private set; }

        public static Container Configure(string allMyBricksOnboardingUrl)
        {
            IoCContainer = new Container();

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
