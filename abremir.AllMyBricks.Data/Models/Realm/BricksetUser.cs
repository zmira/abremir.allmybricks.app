using abremir.AllMyBricks.Data.Enumerations;
using Realms;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    public class BricksetUser : RealmObject
    {
        [PrimaryKey]
        public string BricksetUsername { get; set; }

        [Indexed]
        public byte UserTypeRaw { get; set; }

        public DateTimeOffset? UserSynchronizationTimestamp { get; set; }
        public IList<BricksetUserSet> Sets { get; }

        public BricksetUserTypeEnum UserType {
            get => (BricksetUserTypeEnum)UserTypeRaw;
            set => UserTypeRaw = (byte)value;
        }
    }
}
