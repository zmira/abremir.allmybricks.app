using abremir.AllMyBricks.Data.Enumerations;
using System;

namespace abremir.AllMyBricks.Data.Models
{
    [Obsolete]
    public class RatingItem
    {
        public RatingItemEnum Type { get; set; }
        public byte Value { get; set; }
    }
}
