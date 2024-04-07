using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class AdjustingThemesWithDifferencesEnd
    {
        public Dictionary<short, HashSet<string>> AffectedThemes { get; set; }
    }
}
