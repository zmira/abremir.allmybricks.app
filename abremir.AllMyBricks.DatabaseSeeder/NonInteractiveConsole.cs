using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Services;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    public static class NonInteractiveConsole
    {
        public static async Task Run(bool compress)
        {
            await IoC.IoCContainer.GetInstance<ISetSynchronizationService>().SynchronizeAllSets();

            IoC.IoCContainer.GetInstance<IAssetManagementService>().CompactAllMyBricksDatabase();

            if (compress)
            {
                IoC.IoCContainer.GetInstance<IAssetManagementService>().CompressDatabaseFile();
            }
        }
    }
}
