using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class BricksetUserSetExtensions
    {
        public static Managed.BricksetUserSet ToRealmObject(this BricksetUserSet source)
        {
            return new Managed.BricksetUserSet
            {
                LastChangeTimestamp = source.LastChangeTimestamp,
                Owned = source.Owned,
                QuantityOwned = source.QuantityOwned,
                SetId = source.SetId,
                Wanted = source.Wanted
            };
        }

        public static BricksetUserSet ToPlainObject(this Managed.BricksetUserSet source)
        {
            return new BricksetUserSet
            {
                LastChangeTimestamp = source.LastChangeTimestamp,
                Owned = source.Owned,
                QuantityOwned = source.QuantityOwned,
                SetId = source.SetId,
                Wanted = source.Wanted
            };
        }

        public static IEnumerable<BricksetUserSet> ToPlainObjectEnumerable(this IEnumerable<Managed.BricksetUserSet> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }

        public static IEnumerable<Managed.BricksetUserSet> ToRealmObjectEnumerable(this IEnumerable<BricksetUserSet> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }
    }
}
