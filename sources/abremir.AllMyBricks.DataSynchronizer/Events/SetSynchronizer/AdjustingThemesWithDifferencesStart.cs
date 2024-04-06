using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class AdjustingThemesWithDifferencesStart
    {
        public Dictionary<short, HashSet<string>> AffectedThemes { get; set; }
    }
}
