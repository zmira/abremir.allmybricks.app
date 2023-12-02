using System;
using System.Collections.Generic;
using abremir.AllMyBricks.Data.Enumerations;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class BricksetUser
    {
        [BsonId(true)]
        public int Id { get; set; }

        public string BricksetUsername { get; set; }
        public BricksetUserType UserType { get; set; }
        public DateTimeOffset? UserSynchronizationTimestamp { get; set; }

        public IList<BricksetUserSet> Sets { get; set; } = new List<BricksetUserSet>();
    }
}
