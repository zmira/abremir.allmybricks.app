using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SetsAcquired : IDataSynchronizerEvent
    {
        public int Count { get; set; }
        public int? Year { get; set; }
    }
}