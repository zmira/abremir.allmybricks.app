using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOnboardingServices(this IServiceCollection services, string allMyBricksOnboardingUrl)
        {
            Guard.IsNotNull(services);

            return services
                .AddTransient<IApiKeyService>((_) => new ApiKeyService(allMyBricksOnboardingUrl))
                .AddTransient<IRegistrationService>((_) => new RegistrationService(allMyBricksOnboardingUrl))
                .AddTransient<IOnboardingService, OnboardingService>();
        }
    }
}
