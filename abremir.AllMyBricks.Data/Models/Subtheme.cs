using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class Subtheme
    {
        [BsonId(true)]
        public int Id { get; set; }

        [BsonRef]
        public Theme Theme { get; set; }

        public string Name { get; set; }
        public short YearFrom { get; set; }
        public short YearTo { get; set; }
        public short SetCount { get; set; }

        public string SubthemeKey => $"{Theme.Name}-{Name}";
    }
}
