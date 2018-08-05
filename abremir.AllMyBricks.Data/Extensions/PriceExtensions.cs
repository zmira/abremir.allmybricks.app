using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class PriceExtensions
    {
        public static Managed.Price ToRealmObject(this Price source)
        {
            return new Managed.Price
            {
                Region = source.Region,
                Value = source.Value
            };
        }

        public static Price ToPlainObject(this Managed.Price source)
        {
            return new Price
            {
                Region = source.Region,
                Value = source.Value
            };
        }

        public static IEnumerable<Managed.Price> ToRealmObject(this IEnumerable<Price> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<Price> ToPlainObject(this IEnumerable<Managed.Price> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}