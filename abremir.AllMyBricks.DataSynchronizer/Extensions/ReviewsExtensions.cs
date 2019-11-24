using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class ReviewsExtensions
    {
        public static Review ToReview(this Reviews source)
        {
            var review = new Review
            {
                Author = source.Author,
                DatePosted = source.DatePosted,
                ReviewContent = source.Review?.SanitizeBricksetReview(),
                Title = source.Title?.SanitizeBricksetString(),
                Html = source.Html
            };

            review.RatingComponents.Add(new RatingItem
            {
                Type = RatingItemEnum.Overall,
                Value = (byte)source.OverallRating
            });

            review.RatingComponents.Add(new RatingItem
            {
                Type = RatingItemEnum.Parts,
                Value = (byte)source.Parts
            });

            review.RatingComponents.Add(new RatingItem
            {
                Type = RatingItemEnum.BuildingExperience,
                Value = (byte)source.BuildingExperience
            });

            review.RatingComponents.Add(new RatingItem
            {
                Type = RatingItemEnum.Playability,
                Value = (byte)source.Playability
            });

            review.RatingComponents.Add(new RatingItem
            {
                Type = RatingItemEnum.ValueForMoney,
                Value = (byte)source.ValueForMoney
            });

            return review;
        }

        public static IEnumerable<Review> ToReviewEnumerable(this IEnumerable<Reviews> source)
        {
            foreach (var item in source)
            {
                yield return item.ToReview();
            }
        }
    }
}
