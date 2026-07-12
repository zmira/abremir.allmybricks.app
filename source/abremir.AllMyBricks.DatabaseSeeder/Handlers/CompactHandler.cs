using System.Threading.Tasks;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class CompactHandler
    {
        public static async Task<int> Run(IHost host)
        {
            return await host.Services.GetRequiredService<IAssetManagementService>().CompactAllMyBricksDatabase().ConfigureAwait(false);
        }
    }
}
