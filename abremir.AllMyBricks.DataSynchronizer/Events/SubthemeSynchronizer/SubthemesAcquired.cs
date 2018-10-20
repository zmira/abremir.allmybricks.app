using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SubthemesAcquired : IDataSynchronizerEvent
    {
        public string Theme { get; set; }
        public int Count { get; set; }
    }
}