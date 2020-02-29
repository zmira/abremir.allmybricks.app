using LiteDB;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Models
{
    public class Theme
    {
        [BsonId(true)]
        public int Id { get; set; }

        public string Name { get; set; }
        public short YearFrom { get; set; }
        public short YearTo { get; set; }
        public short SetCount { get; set; }
        public short SubthemeCount { get; set; }

        public IList<YearSetCount> SetCountPerYear { get; set; } = new List<YearSetCount>();
    }
}
