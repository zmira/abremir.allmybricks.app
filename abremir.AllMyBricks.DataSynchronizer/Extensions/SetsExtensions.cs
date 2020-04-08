using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class SetsExtensions
    {
        public static Set ToSet(this Sets source)
        {
            return new Set
            {
                SetId = source.SetId,
                Number = source.Number,
                Name = source.Name,
                Description = source.ExtendedData?.Description?.SanitizeBricksetString(),
                NumberVariant = (byte)source.NumberVariant,
                Year = (short)source.Year,
                Pieces = (short?)source.Pieces,
                Minifigs = (short?)source.Minifigs,
                BricksetUrl = source.BricksetUrl?.SanitizeBricksetString(),
                Released = source.Released,
                LastUpdated = source.LastUpdated,
                OwnedByTotal = (short)(source.Collections?.OwnedBy ?? 0),
                WantedByTotal = (short)(source.Collections?.WantedBy ?? 0),
                Rating = source.Rating,
                EAN = string.IsNullOrWhiteSpace(source.Barcode?.EAN) ? null : source.Barcode.EAN,
                UPC = string.IsNullOrWhiteSpace(source.Barcode?.UPC) ? null : source.Barcode.UPC,
                Availability = string.IsNullOrWhiteSpace(source.Availability) ? null : source.Availability,
                AgeMin = (byte?)source.AgeRange?.Min,
                AgeMax = (byte?)source.AgeRange?.Max,
                Height = source.Dimensions?.Height,
                Width = source.Dimensions?.Width,
                Depth = source.Dimensions?.Depth,
                Weight = source.Dimensions?.Weight,
                Notes = source.ExtendedData?.Notes?.SanitizeBricksetString()
            };
        }
    }
}
