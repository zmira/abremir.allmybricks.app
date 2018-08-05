using abremir.AllMyBricks.Data.Models;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class CategoryExtensions
    {
        public static Managed.Category ToRealmObject(this Category source)
        {
            return new Managed.Category
            {
                Value = source.Value
            };
        }

        public static Category ToPlainObject(this Managed.Category source)
        {
            return new Category
            {
                Value = source.Value
            };
        }
    }
}