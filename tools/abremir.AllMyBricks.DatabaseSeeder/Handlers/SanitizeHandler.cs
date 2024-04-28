using System.Threading.Tasks;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class SanitizeHandler
    {
        public static async Task Run(IHost host)
        {
            await host.Services.GetService<ISetSanitizeService>().Synchronize().ConfigureAwait(false);
        }
    }
}
