using Realms;

namespace abremir.AllMyBricks.Data.Models
{
    public class Subtheme : RealmObject
    {
        [PrimaryKey]
        public string Name { get; set; }

        [Indexed]
        public short YearFrom { get; set; }

        [Indexed]
        public short YearTo { get; set; }

        public Theme Theme { get; set; }
        public short SetCount { get; set; }
    }
}