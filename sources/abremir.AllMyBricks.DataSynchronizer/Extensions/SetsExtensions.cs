using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class SetsExtensions
    {
        public static Set ToSet(this Sets source)
        {
            var set = new Set
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
                Rating = source.Rating,
                Availability = string.IsNullOrWhiteSpace(source.Availability) ? null : source.Availability,
                Notes = source.ExtendedData?.Notes?.SanitizeBricksetString(),
                Totals = new SetTotals
                {
                    OwnedBy = (short)(source.Collections?.OwnedBy ?? 0),
                    WantedBy = (short)(source.Collections?.WantedBy ?? 0)
                },
                AgeRange = new Data.Models.SetAgeRange
                {
                    Min = (byte?)source.AgeRange.Min,
                    Max = (byte?)source.AgeRange.Max
                },
                Barcodes = [],
                Dimensions = new Dimensions
                {
                    Height = source.Dimensions?.Height,
                    Width = source.Dimensions?.Width,
                    Depth = source.Dimensions?.Depth,
                    Weight = source.Dimensions?.Weight
                }
            };

            if (!string.IsNullOrWhiteSpace(source.Barcode?.EAN))
            {
                set.Barcodes.Add(new Barcode { Type = BarcodeType.EAN, Value = source.Barcode.EAN });
            }

            if (!string.IsNullOrWhiteSpace(source.Barcode?.UPC))
            {
                set.Barcodes.Add(new Barcode { Type = BarcodeType.UPC, Value = source.Barcode.UPC });
            }

            return set;
        }
    }
}
