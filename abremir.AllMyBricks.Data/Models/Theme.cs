using System.Collections.Generic;
using System.Diagnostics;

namespace abremir.AllMyBricks.Data.Models
{
    [DebuggerStepThrough]
    public class Theme
    {
        public string Name { get; set; }
        public short YearFrom { get; set; }
        public short YearTo { get; set; }
        public short SetCount { get; set; }
        public short SubthemeCount { get; set; }

        public IList<YearSetCount> SetCountPerYear { get; set; } = new List<YearSetCount>();
    }
}