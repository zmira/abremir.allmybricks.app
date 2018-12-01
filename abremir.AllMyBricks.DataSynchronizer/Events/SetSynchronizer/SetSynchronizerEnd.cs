using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SetSynchronizerEnd : IDataSynchronizerEvent
    {
        public bool ForSubtheme { get; set; }
    }
}