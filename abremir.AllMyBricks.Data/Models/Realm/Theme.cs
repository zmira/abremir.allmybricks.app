using Realms;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    internal class Theme : RealmObject
    {
        [PrimaryKey]
        public string Name { get; set; }

        [Indexed]
        public short YearFrom { get; set; }

        [Indexed]
        public short YearTo { get; set; }

        public short SetCount { get; set; }
        public short SubthemeCount { get; set; }
        public IList<YearSetCount> SetCountPerYear { get; }
    }
}