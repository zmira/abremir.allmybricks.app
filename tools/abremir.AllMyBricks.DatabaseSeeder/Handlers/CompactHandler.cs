using System.Threading.Tasks;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class CompactHandler
    {
        public static async Task Run(IHost host)
        {
            await host.Services.GetService<IAssetManagementService>().CompactAllMyBricksDatabase().ConfigureAwait(false);
        }
    }
}
