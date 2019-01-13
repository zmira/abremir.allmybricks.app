using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    public static class NonInteractiveConsole
    {
        public static async Task Run()
        {
            var dataSynchronizationService = IoC.IoCContainer.GetInstance<IDataSynchronizationService>();
            await dataSynchronizationService.SynchronizeAllSetData();
        }
    }
}
