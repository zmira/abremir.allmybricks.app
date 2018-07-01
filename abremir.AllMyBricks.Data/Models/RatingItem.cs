using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.Data.Models
{
    public class RatingItem
    {
        public RatingItemEnum Type { get; set; }
        public byte Value { get; set; }
    }
}