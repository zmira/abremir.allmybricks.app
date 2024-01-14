using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using CommunityToolkit.Diagnostics;
using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOnboardingServices(this IServiceCollection services, string allMyBricksOnboardingUrl)
        {
            Guard.IsNotNull(services);

            return services
                .AddSingleton<IFlurlClientCache>(_ => new FlurlClientCache()
                    .Add(Constants.AllMyBricksOnboardingUrlFlurlClientCacheName, allMyBricksOnboardingUrl, (builder) => builder.Settings.JsonSerializer = OnboardingJsonSerializer.JsonSerializer)
                    .Add(Constants.AllMyBricksOnboardingHmacUrlFlurlClientCacheName, allMyBricksOnboardingUrl, (builder) =>
                    {
                        builder.Settings.JsonSerializer = OnboardingJsonSerializer.JsonSerializer;
                        builder.AddMiddleware(() => new HmacDelegatingHandler());
                    }))
                .AddTransient<IApiKeyService, ApiKeyService>()
                .AddTransient<IRegistrationService, RegistrationService>()
                .AddTransient<IOnboardingService, OnboardingService>();
        }
    }
}
