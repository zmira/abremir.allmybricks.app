using System.Collections.Generic;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class AdditionalImagesExtensions
    {
        public static Image ToImage(this SetImage source)
        {
            return new Image
            {
                ImageUrl = source.ImageUrl?.SanitizeBricksetString(),
                ThumbnailUrl = source.ThumbnailUrl?.SanitizeBricksetString()
            };
        }

        public static IEnumerable<Image> ToImageEnumerable(this IEnumerable<SetImage> source)
        {
            foreach (var item in source)
            {
                yield return item.ToImage();
            }
        }
    }
}
