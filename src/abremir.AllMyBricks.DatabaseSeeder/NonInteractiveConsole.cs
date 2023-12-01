using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    public static class NonInteractiveConsole
    {
        public static async Task Run(IHost host, IList<string> synchronizationContext, bool compress, bool uncompress, bool encrypted)
        {
            if (!synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueNone))
            {
                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueSets)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
                {
                    await host.Services.GetService<ISetSynchronizationService>().SynchronizeAllSets().ConfigureAwait(false);
                }

                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValuePrimaryUsers)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
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

                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueFriends)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
                {
                    await host.Services.GetService<IUserSynchronizationService>().SynchronizeBricksetFriendsSets().ConfigureAwait(false);
                }

                host.Services.GetService<IAssetManagementService>().CompactAllMyBricksDatabase();
            }

            if (compress)
            {
                host.Services.GetService<IAssetManagementService>().CompressDatabaseFile(encrypted);
            }

            if (uncompress)
            {
                host.Services.GetService<IAssetManagementService>().UncompressDatabaseFile(encrypted);
            }
        }
    }
}
