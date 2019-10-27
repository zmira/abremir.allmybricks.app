using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Globalization;

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
                Description = source.Description?.SanitizeBricksetString(),
                NumberVariant = (byte)source.NumberVariant,
                Year = string.IsNullOrWhiteSpace(source.Year) ? (short?)null : short.Parse(source.Year),
                Pieces = string.IsNullOrWhiteSpace(source.Pieces) ? (short?)null : short.Parse(source.Pieces),
                Minifigs = string.IsNullOrWhiteSpace(source.Minifigs) ? (short?)null : short.Parse(source.Minifigs),
                BricksetUrl = source.BricksetUrl?.SanitizeBricksetString(),
                Released = source.Released,
                LastUpdated = source.LastUpdated,
                OwnedByTotal = (short)source.OwnedByTotal,
                WantedByTotal = (short)source.WantedByTotal,
                Rating = (float)source.Rating,
                Ean = string.IsNullOrWhiteSpace(source.Ean) ? null : source.Ean,
                Upc = string.IsNullOrWhiteSpace(source.Upc) ? null : source.Upc,
                Availability = string.IsNullOrWhiteSpace(source.Availability) ? null : source.Availability,
                AgeMin = string.IsNullOrWhiteSpace(source.AgeMin) ? (byte?)null : byte.Parse(source.AgeMin),
                AgeMax = string.IsNullOrWhiteSpace(source.AgeMax) ? (byte?)null : byte.Parse(source.AgeMax),
                Height = string.IsNullOrWhiteSpace(source.Height) ? (float?)null : float.Parse(source.Height, NumberStyles.Any, CultureInfo.InvariantCulture),
                Width = string.IsNullOrWhiteSpace(source.Width) ? (float?)null : float.Parse(source.Width, NumberStyles.Any, CultureInfo.InvariantCulture),
                Depth = string.IsNullOrWhiteSpace(source.Depth) ? (float?)null : float.Parse(source.Depth, NumberStyles.Any, CultureInfo.InvariantCulture),
                Weight = string.IsNullOrWhiteSpace(source.Weight) ? (float?)null : float.Parse(source.Weight, NumberStyles.Any, CultureInfo.InvariantCulture),
                Notes = source.Notes?.SanitizeBricksetString()
            };
        }
    }
}
