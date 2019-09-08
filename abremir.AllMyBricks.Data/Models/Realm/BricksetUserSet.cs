using Realms;
using System;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    public class BricksetUserSet : RealmObject
    {
        public long SetId { get; set; }
        public bool Wanted { get; set; }
        public bool Owned { get; set; }
        public short QuantityOwned { get; set; }
        public DateTimeOffset? LastChangeTimestamp { get; set; }
    }
}
