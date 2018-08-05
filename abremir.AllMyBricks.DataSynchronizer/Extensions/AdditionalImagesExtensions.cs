using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class AdditionalImagesExtensions
    {
        public static Image ToImage(this AdditionalImages source)
        {
            return new Image
            {
                ImageUrl = source.ImageUrl.SanitizeUrl(),
                LargeThumbnailUrl = source.LargeThumbnailUrl.SanitizeUrl(),
                ThumbnailUrl = source.ThumbnailUrl.SanitizeUrl()
            };
        }

        public static IEnumerable<Image> ToImageEnumerable(this IEnumerable<AdditionalImages> source)
        {
            foreach (var item in source)
            {
                yield return item.ToImage();
            }
        }
    }
}