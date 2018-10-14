using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SynchronizedSubtheme : IDataSynchronizerEvent
    {
        public string Name { get; set; }
    }
}