using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SynchronizingSubtheme : IDataSynchronizerEvent
    {
        public string Theme { get; set; }
        public string Subtheme { get; set; }
    }
}