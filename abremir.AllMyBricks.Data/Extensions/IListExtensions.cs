using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                list.Add(item);
            }
        }
    }
}