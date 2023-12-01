using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Services;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.Data.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            Guard.IsNotNull(services);

            return services
                .AddTransient<IRepositoryService, RepositoryService>()
                .AddTransient<IThemeRepository, ThemeRepository>()
                .AddTransient<ISubthemeRepository, SubthemeRepository>()
                .AddTransient<IReferenceDataRepository, ReferenceDataRepository>()
                .AddTransient<ISetRepository, SetRepository>()
                .AddTransient<IInsightsRepository, InsightsRepository>()
                .AddTransient<IBricksetUserRepository, BricksetUserRepository>();
        }
    }
}
