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
                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueSets))
                {
                    await IoC.IoCContainer.GetInstance<ISetSynchronizationService>().SynchronizeAllSets();
                }

                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValuePrimaryUsers)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
                {
                    await IoC.IoCContainer.GetInstance<IUserSynchronizationService>().SynchronizeBricksetPrimaryUsersSets();
                }

                if (synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueFriends)
                    || synchronizationContext.Contains(DatabaseSeederConstants.DatasetValueAll))
                {
                    await IoC.IoCContainer.GetInstance<IUserSynchronizationService>().SynchronizeBricksetFriendsSets();
                }
            }

            IoC.IoCContainer.GetInstance<IAssetManagementService>().CompactAllMyBricksDatabase();

            if (compress)
            {
                IoC.IoCContainer.GetInstance<IAssetManagementService>().CompressDatabaseFile();
            }
        }
    }
}
