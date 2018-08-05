using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class ImageExtensions
    {
        public static Managed.Image ToRealmObject(this Image source)
        {
            return new Managed.Image
            {
                ImageUrl = source.ImageUrl,
                LargeThumbnailUrl = source.LargeThumbnailUrl,
                ThumbnailUrl = source.ThumbnailUrl
            };
        }

        public static Image ToPlainObject(this Managed.Image source)
        {
            return new Image
            {
                ImageUrl = source.ImageUrl,
                LargeThumbnailUrl = source.LargeThumbnailUrl,
                ThumbnailUrl = source.ThumbnailUrl
            };
        }

        public static IEnumerable<Managed.Image> ToRealmObjectEnumerable(this IEnumerable<Image> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<Image> ToPlainObjectEnumerable(this IEnumerable<Managed.Image> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}
