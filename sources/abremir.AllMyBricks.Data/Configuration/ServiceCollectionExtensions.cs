﻿using abremir.AllMyBricks.Data.Interfaces;
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
                .AddSingleton<IRepositoryService, RepositoryService>()
                .AddSingleton<IThemeRepository, ThemeRepository>()
                .AddSingleton<ISubthemeRepository, SubthemeRepository>()
                .AddSingleton<IReferenceDataRepository, ReferenceDataRepository>()
                .AddSingleton<ISetRepository, SetRepository>()
                .AddSingleton<IInsightsRepository, InsightsRepository>()
                .AddSingleton<IBricksetUserRepository, BricksetUserRepository>()
                .AddSingleton<IMigrationRunner, MigrationRunner>();
        }
    }
}
