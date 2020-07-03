using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using LightInject;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(string allMyBricksOnboardingUrl, IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<IApiKeyService>((_) => new ApiKeyService(allMyBricksOnboardingUrl));
            container.Register<IRegistrationService>((_) => new RegistrationService(allMyBricksOnboardingUrl));
            container.Register<IOnboardingService, OnboardingService>();

            return container;
        }
    }
}
