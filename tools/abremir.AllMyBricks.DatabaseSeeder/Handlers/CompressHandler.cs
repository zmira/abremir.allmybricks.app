using abremir.AllMyBricks.DatabaseSeeder.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class CompressHandler
    {
        public static void Run(IHost host, bool encrypted)
        {
            host.Services.GetService<IAssetManagementService>().CompressDatabaseFile(encrypted);
        }
    }
}
