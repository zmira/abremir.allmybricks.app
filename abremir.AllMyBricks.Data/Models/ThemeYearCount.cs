using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class ThemeYearCount
    {
        [BsonId(false)]
        public ThemeYear Key { get; set; }
        public ushort Count { get; set; }

        public Theme Theme => Key.Theme;
        public ushort Year => Key.Year;
    }
}