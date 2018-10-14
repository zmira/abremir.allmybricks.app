using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer
{
    public class ThemesAcquired : IDataSynchronizerEvent
    {
        public int Count { get; set; }
    }
}