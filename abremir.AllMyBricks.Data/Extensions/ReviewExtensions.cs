using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class ReviewExtensions
    {
        public static Managed.Review ToRealmObject(this Review source)
        {
            var review = new Managed.Review
            {
                Author = source.Author,
                DatePosted = source.DatePosted,
                Html = source.Html,
                ReviewContent = source.ReviewContent,
                Title = source.Title
            };

            review.RatingComponents.AddRange((source.RatingComponents ?? new List<RatingItem>()).ToRealmObjectEnumerable());

            return review;
        }

        public static Review ToPlainObject(this Managed.Review source)
        {
            var review = new Review
            {
                Author = source.Author,
                DatePosted = source.DatePosted,
                Html = source.Html,
                ReviewContent = source.ReviewContent,
                Title = source.Title
            };

            review.RatingComponents.AddRange((source.RatingComponents ?? new List<Managed.RatingItem>()).ToPlainObjectEnumerable());

            return review;
        }

        public static IEnumerable<Managed.Review> ToRealmObjectEnumerable(this IEnumerable<Review> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<Review> ToPlainObjectEnumerable(this IEnumerable<Managed.Review> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}
