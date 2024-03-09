using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.DataSynchronizer.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataSynchronizerServices(this IServiceCollection services)
        {
            Guard.IsNotNull(services);

            return services
                .AddTransient<ISetSynchronizationService, SetSynchronizationService>()
                .AddTransient<IThemeSynchronizer, ThemeSynchronizer>()
                .AddTransient<ISubthemeSynchronizer, SubthemeSynchronizer>()
                .AddTransient<IFullSetSynchronizer, FullSetSynchronizer>()
                .AddTransient<IPartialSetSynchronizer, PartialSetSynchronizer>()
                .AddTransient<IThumbnailSynchronizer, ThumbnailSynchronizer>()
                .AddTransient<IUserSynchronizationService, UserSynchronizationService>()
                .AddTransient<IUserSynchronizer, UserSynchronizer>();
        }
    }
}
