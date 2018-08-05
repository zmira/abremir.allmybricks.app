using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class SubthemeExtensions
    {
        public static Managed.Subtheme ToRealmObject(this Subtheme source)
        {
            return new Managed.Subtheme
            {
                Name = source.Name,
                SetCount = source.SetCount,
                SubthemeKey = $"{source.Name}-{source.Theme.Name}",
                Theme = source.Theme?.ToRealmObject(),
                YearFrom = source.YearFrom,
                YearTo = source.YearTo
            };
        }

        public static Subtheme ToPlainObject(this Managed.Subtheme source)
        {
            return new Subtheme
            {
                Name = source.Name,
                SetCount = source.SetCount,
                Theme = source.Theme?.ToPlainObject(),
                YearFrom = source.YearFrom,
                YearTo = source.YearTo
            };
        }

        public static IEnumerable<Subtheme> ToPlainObjectEnumerable(this IEnumerable<Managed.Subtheme> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}