using abremir.AllMyBricks.DataSynchronizer.Interfaces;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer
{
    public class SynchronizingThumbnail : IDataSynchronizerEvent
    {
        public string Thumbnail { get; set; }
    }
}