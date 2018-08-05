using Realms;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    public class Subtheme : RealmObject
    {
        [PrimaryKey]
        public string SubthemeKey { get; set; }

        [Indexed]
        public short YearFrom { get; set; }

        [Indexed]
        public short YearTo { get; set; }

        [Indexed]
        public string Name { get; set; }

        public Theme Theme { get; set; }
        public short SetCount { get; set; }
    }
}