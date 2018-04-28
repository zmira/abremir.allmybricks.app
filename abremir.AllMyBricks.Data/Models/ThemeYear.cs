using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class ThemeYear
    {
        [BsonRef]
        public Theme Theme { get; set; }
        public ushort Year { get; set; }
    }
}