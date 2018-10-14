using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService
{
    public class ProcessingTheme : IDataSynchronizerEvent
    {
        public string Name { get; set; }
    }
}