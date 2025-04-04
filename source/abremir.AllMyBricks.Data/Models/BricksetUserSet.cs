using System;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;
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

        private DateTimeOffset? _lastChangeTimestamp;
        public DateTimeOffset? LastChangeTimestamp
        {
            get { return _lastChangeTimestamp; }
            set { _lastChangeTimestamp = value?.ToHundredthOfSecond(); }
        }
    }
}
