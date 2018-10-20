using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer
{
    public class SynchronizingTheme : IDataSynchronizerEvent
    {
        public string Theme { get; set; }
    }
}