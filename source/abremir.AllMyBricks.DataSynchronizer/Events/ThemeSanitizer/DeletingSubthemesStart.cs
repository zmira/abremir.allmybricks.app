using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSanitizer
{
    public class DeletingSubthemesStart
    {
        public string AffectedTheme { get; set; }
        public List<string> AffectedSubthemes { get; set; }
    }
}
