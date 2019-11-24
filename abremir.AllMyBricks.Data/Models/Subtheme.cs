namespace abremir.AllMyBricks.Data.Models
{
    public class Subtheme
    {
        public string Name { get; set; }
        public short YearFrom { get; set; }
        public short YearTo { get; set; }
        public Theme Theme { get; set; }
        public short SetCount { get; set; }
    }
}
