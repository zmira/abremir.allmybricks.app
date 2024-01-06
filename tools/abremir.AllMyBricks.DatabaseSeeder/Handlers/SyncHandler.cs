using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder.Handlers
{
    internal static class SyncHandler
    {
        public static async Task Run(IHost host, IList<Dataset> synchronizationContext)
        {
            if (synchronizationContext.Contains(Dataset.Sets)
                || synchronizationContext.Contains(Dataset.All))
            {
                await host.Services.GetService<ISetSynchronizationService>().SynchronizeAllSets().ConfigureAwait(false);
            }

            if (synchronizationContext.Contains(Dataset.PrimaryUsers)
                || synchronizationContext.Contains(Dataset.All))
            {
                var userRepository = host.Services.GetService<IBricksetUserRepository>();

                foreach (var primaryUser in Settings.BricksetPrimaryUsers)
                {
                    if (userRepository.Get(primaryUser.Key) is null)
                    {
                        userRepository.Add(BricksetUserType.Primary, primaryUser.Key);
                    }
                }

                await host.Services.GetService<IUserSynchronizationService>().SynchronizeBricksetPrimaryUsersSets().ConfigureAwait(false);
            }

            if (synchronizationContext.Contains(Dataset.Friends)
                || synchronizationContext.Contains(Dataset.All))
            {
                await host.Services.GetService<IUserSynchronizationService>().SynchronizeBricksetFriendsSets().ConfigureAwait(false);
            }

            host.Services.GetService<IAssetManagementService>().CompactAllMyBricksDatabase();
        }
    }
}
