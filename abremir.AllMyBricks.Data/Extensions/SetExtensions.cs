using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class SetExtensions
    {
        public static Managed.Set ToRealmObject(this Set source)
        {
            var set = new Managed.Set
            {
                AgeMax = source.AgeMax,
                AgeMin = source.AgeMin,
                Availability = source.Availability,
                BricksetUrl = source.BricksetUrl,
                Category = source.Category?.ToRealmObject(),
                Depth = source.Depth,
                Description = source.Description,
                Ean = source.Ean,
                Height = source.Height,
                LastUpdated = source.LastUpdated,
                Minifigs = source.Minifigs,
                Name = source.Name,
                Notes = source.Notes,
                Number = source.Number,
                NumberVariant = source.NumberVariant,
                OwnedByTotal = source.OwnedByTotal,
                PackagingType = source.PackagingType?.ToRealmObject(),
                Pieces = source.Pieces,
                Rating = source.Rating,
                Released = source.Released,
                SetId = source.SetId,
                Subtheme = source.Subtheme?.ToRealmObject(),
                Theme = source.Theme?.ToRealmObject(),
                ThemeGroup = source.ThemeGroup?.ToRealmObject(),
                Upc = source.Upc,
                UserRating = source.UserRating,
                WantedByTotal = source.WantedByTotal,
                Weight = source.Weight,
                Width = source.Width,
                Year = source.Year
            };

            set.Images.AddRange((source.Images ?? new List<Image>()).ToRealmObject());
            set.Instructions.AddRange((source.Instructions ?? new List<Instruction>()).ToRealmObject());
            set.Prices.AddRange((source.Prices ?? new List<Price>()).ToRealmObject());
            set.Reviews.AddRange((source.Reviews ?? new List<Review>()).ToRealmObject());
            set.Tags.AddRange((source.Tags ?? new List<Tag>()).ToRealmObject());

            return set;
        }

        public static Set ToPlainObject(this Managed.Set source)
        {
            var set = new Set
            {
                AgeMax = source.AgeMax,
                AgeMin = source.AgeMin,
                Availability = source.Availability,
                BricksetUrl = source.BricksetUrl,
                Category = source.Category?.ToPlainObject(),
                Depth = source.Depth,
                Description = source.Description,
                Ean = source.Ean,
                Height = source.Height,
                LastUpdated = source.LastUpdated,
                Minifigs = source.Minifigs,
                Name = source.Name,
                Notes = source.Notes,
                Number = source.Number,
                NumberVariant = source.NumberVariant,
                OwnedByTotal = source.OwnedByTotal,
                PackagingType = source.PackagingType?.ToPlainObject(),
                Pieces = source.Pieces,
                Rating = source.Rating,
                Released = source.Released,
                SetId = source.SetId,
                Subtheme = source.Subtheme?.ToPlainObject(),
                Theme = source.Theme?.ToPlainObject(),
                ThemeGroup = source.ThemeGroup?.ToPlainObject(),
                Upc = source.Upc,
                UserRating = source.UserRating,
                WantedByTotal = source.WantedByTotal,
                Weight = source.Weight,
                Width = source.Width,
                Year = source.Year
            };

            set.Images.AddRange((source.Images ?? new List<Managed.Image>()).ToPlainObject());
            set.Instructions.AddRange((source.Instructions ?? new List<Managed.Instruction>()).ToPlainObject());
            set.Prices.AddRange((source.Prices ?? new List<Managed.Price>()).ToPlainObject());
            set.Reviews.AddRange((source.Reviews ?? new List<Managed.Review>()).ToPlainObject());
            set.Tags.AddRange((source.Tags ?? new List<Managed.Tag>()).ToPlainObject());

            return set;
        }

        public static IEnumerable<Managed.Set> ToRealmObject(this IEnumerable<Set> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<Set> ToPlainObject(this IEnumerable<Managed.Set> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}