using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class YearsExtension
    {
        public static YearSetCount ToYearSetCount(this Years source)
        {
            return new YearSetCount
            {
                Year = short.Parse(source.Year),
                SetCount = (short)source.SetCount
            };
        }

        public static IEnumerable<YearSetCount> ToYearSetCount(this IEnumerable<Years> source)
        {
            foreach (var item in source)
            {
                yield return item.ToYearSetCount();
            }
        }
    }
}