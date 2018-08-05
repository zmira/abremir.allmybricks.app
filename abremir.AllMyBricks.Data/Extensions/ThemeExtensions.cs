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

            theme.SetCountPerYear.AddRange((source.SetCountPerYear ?? new List<YearSetCount>()).ToRealmObjectEnumerable());

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

            theme.SetCountPerYear.AddRange((source.SetCountPerYear ?? new List<Managed.YearSetCount>()).ToPlainObjectEnumerable());

            return theme;
        }

        public static IEnumerable<Theme> ToPlainObjectEnumerable(this IEnumerable<Managed.Theme> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}