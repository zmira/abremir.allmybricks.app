using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class RatingItemExtensions
    {
        public static Managed.RatingItem ToRealmObject(this RatingItem source)
        {
            return new Managed.RatingItem
            {
                Type = source.Type,
                Value = source.Value
            };
        }

        public static RatingItem ToPlainObject(this Managed.RatingItem source)
        {
            return new RatingItem
            {
                Type = source.Type,
                Value = source.Value
            };
        }

        public static IEnumerable<Managed.RatingItem> ToRealmObjectEnumerable(this IEnumerable<RatingItem> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<RatingItem> ToPlainObjectEnumerable(this IEnumerable<Managed.RatingItem> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}