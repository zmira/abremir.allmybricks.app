using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    public static class NonInteractiveConsole
    {
        public static async Task Run(IList<string> synchronizationContext, bool compress)
        {
            if (!synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueNone))
            {
                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueSets)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
                {
                    await IoC.IoCContainer.GetInstance<ISetSynchronizationService>().SynchronizeAllSets();
                }

                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValuePrimaryUsers)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
                {
                    var userRepository = IoC.IoCContainer.GetInstance<IBricksetUserRepository>();

                    foreach (var primaryUser in Settings.BricksetPrimaryUsers)
                    {
                        if (userRepository.Get(primaryUser.Key) == null)
                        {
                            userRepository.Add(BricksetUserTypeEnum.Primary, primaryUser.Key);
                        }
                    }

                    await IoC.IoCContainer.GetInstance<IUserSynchronizationService>().SynchronizeBricksetPrimaryUsersSets();
                }

                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueFriends)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
                {
                    await IoC.IoCContainer.GetInstance<IUserSynchronizationService>().SynchronizeBricksetFriendsSets();
                }

                IoC.IoCContainer.GetInstance<IAssetManagementService>().CompactAllMyBricksDatabase();
            }

            if (compress)
            {
                IoC.IoCContainer.GetInstance<IAssetManagementService>().CompressDatabaseFile();
            }
        }
    }
}
