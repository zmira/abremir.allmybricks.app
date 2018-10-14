using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService
{
    public class ProcessingSubtheme : IDataSynchronizerEvent
    {
        public string Name { get; set; }
    }
}