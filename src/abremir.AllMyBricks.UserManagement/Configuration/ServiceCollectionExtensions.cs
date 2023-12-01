using abremir.AllMyBricks.UserManagement.Interfaces;
using abremir.AllMyBricks.UserManagement.Services;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.UserManagement.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserManagementServices(this IServiceCollection services)
        {
            Guard.IsNotNull(services);

            return services
                .AddTransient<IUserService, UserService>();
        }
    }
}
