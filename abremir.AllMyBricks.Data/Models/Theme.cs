using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class Theme
    {
        [BsonId(false)]
        public string Name { get; set; }
        public ushort SetCount { get; set; }
        public ushort SubthemeCount { get; set; }
        public ushort YearFrom { get; set; }
        public ushort YearTo { get; set; }
    }
}