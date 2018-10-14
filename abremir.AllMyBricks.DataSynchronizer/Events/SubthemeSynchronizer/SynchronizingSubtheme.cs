using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SynchronizingSubtheme : IDataSynchronizerEvent
    {
        public string Name { get; set; }
    }
}