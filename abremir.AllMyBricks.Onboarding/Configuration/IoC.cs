using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using SimpleInjector;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class IoC
    {
        public static Container Configure(string allMyBricksOnboardingUrl, Container container = null)
        {
            container = container ?? new Container();

            container.Register<IApiKeyService>(() => new ApiKeyService(allMyBricksOnboardingUrl), Lifestyle.Transient);
            container.Register<IRegistrationService>(() => new RegistrationService(allMyBricksOnboardingUrl), Lifestyle.Transient);
            container.Register<IOnboardingService, OnboardingService>(Lifestyle.Transient);

            return container;
        }
    }
}
