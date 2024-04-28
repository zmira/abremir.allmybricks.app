using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Events.ThemeSanitizer
{
    public class DeletingThemesStart
    {
        public List<string> AffectedThemes { get; set; }
    }
}
