using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    public static class NonInteractiveConsole
    {
        public static async Task Run(bool createDistributionFile)
        {
            await IoC.IoCContainer.GetInstance<IDataSynchronizationService>().SynchronizeAllSetData();

            IoC.IoCContainer.GetInstance<IAssetManagementService>().CompactAllMyBricksDatabase();

            if (createDistributionFile)
            {
                IoC.IoCContainer.GetInstance<IAssetManagementService>().CompressDatabaseFile();
            }
        }
    }
}
