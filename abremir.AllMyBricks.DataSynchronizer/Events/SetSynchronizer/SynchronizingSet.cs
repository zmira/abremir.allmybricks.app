using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SynchronizingSet : IDataSynchronizerEvent
    {
        public string Identifier { get; set; }
    }
}