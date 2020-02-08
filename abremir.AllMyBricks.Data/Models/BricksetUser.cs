using abremir.AllMyBricks.Data.Enumerations;
using LiteDB;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Models
{
    public class BricksetUser
    {
        [BsonId(false)]
        public string BricksetUsername { get; set; }

        public BricksetUserTypeEnum UserType { get; set; }
        public DateTimeOffset? UserSynchronizationTimestamp { get; set; }

        public IList<BricksetUserSet> Sets { get; set; } = new List<BricksetUserSet>();
    }
}
