using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class BricksetUserExtensions
    {
        public static Managed.BricksetUser ToRealmObject(this BricksetUser source)
        {
            var bricksetUser = new Managed.BricksetUser
            {
                BricksetUsername = source.BricksetUsername,
                UserType = source.UserType,
                UserSynchronizationTimestamp = source.UserSynchronizationTimestamp
            };

            bricksetUser.Sets.AddRange((source.Sets ?? new List<BricksetUserSet>()).ToRealmObjectEnumerable());

            return bricksetUser;
        }

        public static BricksetUser ToPlainObject(this Managed.BricksetUser source)
        {
            var bricksetUser = new BricksetUser
            {
                BricksetUsername = source.BricksetUsername,
                UserType = source.UserType,
                UserSynchronizationTimestamp = source.UserSynchronizationTimestamp
            };

            bricksetUser.Sets.AddRange((source.Sets ?? new List<Managed.BricksetUserSet>()).ToPlainObjectEnumerable());

            return bricksetUser;
        }
    }
}
