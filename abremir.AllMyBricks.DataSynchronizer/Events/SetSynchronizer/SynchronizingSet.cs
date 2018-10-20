using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SynchronizingSet : IDataSynchronizerEvent
    {
        public string Theme { get; set; }
        public string Subtheme { get; set; }
        public string Number { get; set; }
        public int NumberVariant { get; set; }
        public string Name { get; set; }
    }
}