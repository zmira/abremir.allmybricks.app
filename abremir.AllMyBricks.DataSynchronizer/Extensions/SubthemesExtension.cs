using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class SubthemesExtension
    {
        public static Subtheme ToSubtheme(this Subthemes source)
        {
            return new Subtheme
            {
                Name = source.Subtheme,
                SetCount = (short)source.SetCount,
                YearFrom = (short)source.YearFrom,
                YearTo = (short)source.YearTo
            };
        }

        public static IEnumerable<Subtheme> ToSubtheme(this IEnumerable<Subthemes> source)
        {
            foreach (var item in source)
            {
                yield return item.ToSubtheme();
            }
        }
    }
}