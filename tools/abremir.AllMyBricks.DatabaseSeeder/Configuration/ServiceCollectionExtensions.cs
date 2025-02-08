using System;
using abremir.AllMyBricks.AssetManagement.Configuration;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.DataSynchronizer.Configuration;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using abremir.AllMyBricks.UserManagement.Configuration;
using Easy.MessageHub;
using Microsoft.Extensions.DependencyInjection;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseSeederServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            return services
                .AddLoggingServices()
                .AddDataSynchronizerServices()
                .AddBricksetServices()
                .AddDataServices()
                .AddAssetManagementServices()
                .AddUserManagementServices()
                .AddOnboardingServices(string.Empty)
                .AddTransient<IPreferencesService, PreferencesService>()
                .AddTransient<ISecureStorageService, SecureStorageService>()
                .AddTransient<IDeviceInformationService, DeviceInformationService>()
                .AddTransient<IAssetManagementService, AssetManagementService>()
                .AddScoped<IFileSystemService, FileSystemService>()
                .AddScoped<IMessageHub, MessageHub>();
        }
    }
}
