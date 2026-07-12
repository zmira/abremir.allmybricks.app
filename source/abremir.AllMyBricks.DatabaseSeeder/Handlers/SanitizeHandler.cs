using System.Threading.Tasks;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class SanitizeHandler
    {
        public static async Task<int> Run(IHost host)
        {
            return await host.Services.GetRequiredService<ISetSanitizeService>().Synchronize().ConfigureAwait(false);
        }
    }
}
