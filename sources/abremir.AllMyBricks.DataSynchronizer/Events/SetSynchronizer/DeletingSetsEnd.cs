using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class DeletingSetsEnd
    {
        public List<long> AffectedSets { get; set; }
    }
}
