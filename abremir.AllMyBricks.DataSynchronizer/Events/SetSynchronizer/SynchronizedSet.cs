using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SynchronizedSet : IDataSynchronizerEvent
    {
        public string Identifier { get; set; }
    }
}