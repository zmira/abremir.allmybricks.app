using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class AcquiringSetsEnd
    {
        public int Count { get; set; }
        public bool Complete { get; set; }
        public DateTimeOffset? From { get; set; }
        public string Years { get; set; }
    }
}
