using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using SimpleInjector;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container = container ?? new Container();

            container.Register<IApiKeyService, ApiKeyService>(Lifestyle.Transient);
            container.Register<IRegistrationService, RegistrationService>(Lifestyle.Transient);

            return container;
        }
    }
}