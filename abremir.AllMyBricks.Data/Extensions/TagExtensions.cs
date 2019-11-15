using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class TagExtensions
    {
        public static Managed.Tag ToRealmObject(this Tag source)
        {
            return new Managed.Tag
            {
                Value = source.Value
            };
        }

        public static Tag ToPlainObject(this Managed.Tag source)
        {
            return new Tag
            {
                Value = source.Value
            };
        }

        public static IEnumerable<Managed.Tag> ToRealmObjectEnumerable(this IEnumerable<Tag> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<Tag> ToPlainObjectEnumerable(this IEnumerable<Managed.Tag> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}
