using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class SyncHandler
    {
        public static async Task<int> Run(IHost host, IList<Dataset> synchronizationContext)
        {
            int result = 0;

            if (synchronizationContext.Contains(Dataset.Sets)
                || synchronizationContext.Contains(Dataset.All))
            {
                result += await host.Services.GetRequiredService<ISetSynchronizationService>().Synchronize().ConfigureAwait(false);
            }

            if (synchronizationContext.Contains(Dataset.PrimaryUsers)
                || synchronizationContext.Contains(Dataset.All))
            {
                var userRepository = host.Services.GetRequiredService<IBricksetUserRepository>();

                foreach (var primaryUser in Settings.BricksetPrimaryUsers)
                {
                    if (userRepository.Get(primaryUser.Key) is null)
                    {
                        await userRepository.Add(BricksetUserType.Primary, primaryUser.Key).ConfigureAwait(false);
                    }
                }

                result += await host.Services.GetRequiredService<IUserSynchronizationService>().SynchronizeBricksetPrimaryUsersSets().ConfigureAwait(false);
            }

            if (synchronizationContext.Contains(Dataset.Friends)
                || synchronizationContext.Contains(Dataset.All))
            {
                result += await host.Services.GetRequiredService<IUserSynchronizationService>().SynchronizeBricksetFriendsSets().ConfigureAwait(false);
            }

            return result;
        }
    }
}
