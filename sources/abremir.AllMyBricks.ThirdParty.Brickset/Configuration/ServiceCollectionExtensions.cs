using System;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBricksetServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            return services
                .AddTransient<IBricksetApiService, BricksetApiService>();
        }
    }
}
