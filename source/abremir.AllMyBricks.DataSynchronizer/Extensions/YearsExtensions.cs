using System.Collections.Generic;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class YearsExtensions
    {
        public static YearSetCount ToYearSetCount(this Years source)
        {
            return new YearSetCount
            {
                Year = short.Parse(source.Year),
                SetCount = (short)source.SetCount
            };
        }

        public static IEnumerable<YearSetCount> ToYearSetCountEnumerable(this IEnumerable<Years> source)
        {
            foreach (var item in source)
            {
                yield return item.ToYearSetCount();
            }
        }
    }
}
