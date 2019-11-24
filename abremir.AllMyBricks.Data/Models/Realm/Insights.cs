using Realms;
using System;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    public class Insights : RealmObject
    {
        public DateTimeOffset? DataSynchronizationTimestamp { get; set; }
    }
}
