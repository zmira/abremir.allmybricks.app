using abremir.AllMyBricks.Data.Models;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class PackagingTypeExtensions
    {
        public static Managed.PackagingType ToRealmObject(this PackagingType source)
        {
            return new Managed.PackagingType
            {
                Value = source.Value
            };
        }

        public static PackagingType ToPlainObject(this Managed.PackagingType source)
        {
            return new PackagingType
            {
                Value = source.Value
            };
        }
    }
}
