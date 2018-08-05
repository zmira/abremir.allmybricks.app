using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class YearSetCountExtensions
    {
        public static Managed.YearSetCount ToRealmObject(this YearSetCount source)
        {
            return new Managed.YearSetCount
            {
                SetCount = source.SetCount,
                Year = source.Year
            };
        }

        public static YearSetCount ToPlainObject(this Managed.YearSetCount source)
        {
            return new YearSetCount
            {
                SetCount = source.SetCount,
                Year = source.Year
            };
        }

        public static IEnumerable<Managed.YearSetCount> ToRealmObjectEnumerable(this IEnumerable<YearSetCount> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<YearSetCount> ToPlainObjectEnumerable(this IEnumerable<Managed.YearSetCount> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}