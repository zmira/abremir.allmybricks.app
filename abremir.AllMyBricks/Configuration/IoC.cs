using SimpleInjector;

namespace abremir.AllMyBricks.Configuration
{
    public static class IoC
    {
        public static Container IoCContainer { get; private set; }

        public static Container Configure()
        {
            IoCContainer = new Container();

            Device.Configuration.IoC.Configure(IoCContainer);
            Data.Configuration.IoC.Configure(IoCContainer);
            ThirdParty.Brickset.Configuration.IoC.Configure(IoCContainer);
            Onboarding.Configuration.IoC.Configure(IoCContainer);
            DataSynchronizer.Configuration.IoC.Configure(IoCContainer);

            return IoCContainer;
        }
    }
}