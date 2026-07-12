using System.Threading.Tasks;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class CompressHandler
    {
        public static async Task<int> Run(IHost host, bool encrypted)
        {
            return await host.Services.GetRequiredService<IAssetManagementService>().CompressDatabaseFile(encrypted);
        }
    }
}
