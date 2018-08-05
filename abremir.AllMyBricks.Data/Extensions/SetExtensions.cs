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

            set.Images.AddRange((source.Images ?? new List<Image>()).ToRealmObjectEnumerable());
            set.Instructions.AddRange((source.Instructions ?? new List<Instruction>()).ToRealmObjectEnumerable());
            set.Prices.AddRange((source.Prices ?? new List<Price>()).ToRealmObjectEnumerable());
            set.Reviews.AddRange((source.Reviews ?? new List<Review>()).ToRealmObjectEnumerable());
            set.Tags.AddRange((source.Tags ?? new List<Tag>()).ToRealmObjectEnumerable());

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

            set.Images.AddRange((source.Images ?? new List<Managed.Image>()).ToPlainObjectEnumerable());
            set.Instructions.AddRange((source.Instructions ?? new List<Managed.Instruction>()).ToPlainObjectEnumerable());
            set.Prices.AddRange((source.Prices ?? new List<Managed.Price>()).ToPlainObjectEnumerable());
            set.Reviews.AddRange((source.Reviews ?? new List<Managed.Review>()).ToPlainObjectEnumerable());
            set.Tags.AddRange((source.Tags ?? new List<Managed.Tag>()).ToPlainObjectEnumerable());

            return set;
        }

        public static IEnumerable<Set> ToPlainObjectEnumerable(this IEnumerable<Managed.Set> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}