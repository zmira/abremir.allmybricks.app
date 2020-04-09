using abremir.AllMyBricks.Data.Enumerations;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.Data.Models
{
    [Obsolete]
    public class Set_V1
    {
        [BsonId(false)]
        public long SetId { get; set; }

        [BsonRef]
        public Theme Theme { get; set; }

        [BsonRef]
        public Subtheme Subtheme { get; set; }

        [BsonRef]
        public ThemeGroup ThemeGroup { get; set; }

        [BsonRef]
        public PackagingType PackagingType { get; set; }

        [BsonRef]
        public Category Category { get; set; }

        [BsonRef]
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        public string Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Ean { get; set; }
        public string Upc { get; set; }
        public byte NumberVariant { get; set; }
        public short? Year { get; set; }
        public short? Pieces { get; set; }
        public short? Minifigs { get; set; }
        public string BricksetUrl { get; set; }
        public bool Released { get; set; }
        public int OwnedByTotal { get; set; }
        public int WantedByTotal { get; set; }
        public float Rating { get; set; }
        public string Availability { get; set; }
        public byte? AgeMin { get; set; }
        public byte? AgeMax { get; set; }
        public float? Height { get; set; }
        public float? Width { get; set; }
        public float? Depth { get; set; }
        public float? Weight { get; set; }
        public string Notes { get; set; }
        public string UserRating { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public IList<Image_V1> Images { get; set; } = new List<Image_V1>();
        public IList<Price> Prices { get; set; } = new List<Price>();
        public IList<Review> Reviews { get; set; } = new List<Review>();
        public IList<Instruction> Instructions { get; set; } = new List<Instruction>();

        [BsonIgnore]
        public string NumberWithVariant => $"{Number}-{NumberVariant}";

        public Set ToV2()
        {
            var set = new Set
            {
                SetId = SetId,
                Theme = Theme,
                Subtheme = Subtheme,
                ThemeGroup = ThemeGroup,
                PackagingType = PackagingType,
                Category = Category,
                Tags = Tags,
                Number = Number,
                Name = Name,
                Description = Description,
                NumberVariant = NumberVariant,
                Year = Year.Value,
                Pieces = Pieces,
                Minifigs = Minifigs,
                BricksetUrl = BricksetUrl,
                Released = Released,
                Rating = Rating,
                Availability = Availability,
                AgeRange = new SetAgeRange
                {
                    Min = AgeMin,
                    Max = AgeMax
                },
                Totals = new SetTotals
                {
                    OwnedBy = OwnedByTotal,
                    WantedBy = WantedByTotal
                },
                Dimensions = new Dimensions
                {
                    Height = Height,
                    Width = Width,
                    Depth = Depth,
                    Weight = Weight
                },
                Barcodes = new List<Barcode>(),
                Notes = Notes,
                LastUpdated = LastUpdated,
                Instructions = Instructions,
                Images = Images.Select(image => new Image { ImageUrl = image.ImageUrl, ThumbnailUrl = image.ThumbnailUrl }).ToList(),
                Prices = new List<Price>()
            };

            if (!string.IsNullOrWhiteSpace(Ean))
            {
                set.Barcodes.Add(new Barcode { Type = BarcodeTypeEnum.EAN, Value = Ean });
            }

            if (!string.IsNullOrWhiteSpace(Upc))
            {
                set.Barcodes.Add(new Barcode { Type = BarcodeTypeEnum.UPC, Value = Upc });
            }

            foreach (var price in Prices)
            {
                if (price.Region == PriceRegionEnum.EU)
                {
                    price.Region = PriceRegionEnum.DE;
                }

                set.Prices.Add(price);
            }

            return set;
        }
    }
}
