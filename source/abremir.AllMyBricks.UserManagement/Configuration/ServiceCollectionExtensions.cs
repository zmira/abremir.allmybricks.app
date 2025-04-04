using System;
using abremir.AllMyBricks.UserManagement.Interfaces;
using abremir.AllMyBricks.UserManagement.Services;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.UserManagement.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserManagementServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            return services
                .AddTransient<IUserService, UserService>();
        }
    }
}
