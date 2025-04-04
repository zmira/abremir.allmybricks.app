using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSanitizer
{
    public class AdjustingThemesWithDifferencesStart
    {
        public Dictionary<short, HashSet<string>> AffectedThemes { get; set; }
    }
}
