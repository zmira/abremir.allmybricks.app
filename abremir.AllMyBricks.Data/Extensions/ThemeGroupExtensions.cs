using abremir.AllMyBricks.Data.Models;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class ThemeGroupExtensions
    {
        public static Managed.ThemeGroup ToRealmObject(this ThemeGroup source)
        {
            return new Managed.ThemeGroup
            {
                Value = source.Value
            };
        }

        public static ThemeGroup ToPlainObject(this Managed.ThemeGroup source)
        {
            return new ThemeGroup
            {
                Value = source.Value
            };
        }
    }
}