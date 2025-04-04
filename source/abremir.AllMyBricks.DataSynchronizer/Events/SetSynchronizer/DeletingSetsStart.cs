using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class DeletingSetsStart
    {
        public List<long> AffectedSets { get; set; }
    }
}
