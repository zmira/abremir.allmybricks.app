using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class ThemeExtensions
    {
        public static Managed.Theme ToRealmObject(this Theme source)
        {
            var theme = new Managed.Theme
            {
                Name = source.Name,
                SetCount = source.SetCount,
                SubthemeCount = source.SubthemeCount,
                YearFrom = source.YearFrom,
                YearTo = source.YearTo
            };

            theme.SetCountPerYear.AddRange((source.SetCountPerYear ?? new List<YearSetCount>()).ToRealmObject());

            return theme;
        }

        public static Theme ToPlainObject(this Managed.Theme source)
        {
            var theme = new Theme
            {
                Name = source.Name,
                SetCount = source.SetCount,
                SubthemeCount = source.SubthemeCount,
                YearFrom = source.YearFrom,
                YearTo = source.YearTo
            };

            theme.SetCountPerYear.AddRange((source.SetCountPerYear ?? new List<Managed.YearSetCount>()).ToPlainObject());

            return theme;
        }

        public static IEnumerable<Managed.Theme> ToRealmObject(this IEnumerable<Theme> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<Theme> ToPlainObject(this IEnumerable<Managed.Theme> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}