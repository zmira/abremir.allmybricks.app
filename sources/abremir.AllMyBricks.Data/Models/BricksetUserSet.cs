using System;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class BricksetUserSet
    {
        [BsonRef]
        public Set Set { get; set; }

        public bool Wanted { get; set; }
        public bool Owned { get; set; }
        public short QuantityOwned { get; set; }
        public DateTimeOffset? LastChangeTimestamp { get; set; }
    }
}
