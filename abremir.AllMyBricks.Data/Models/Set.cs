using LiteDB;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Models
{
    public class Set
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
        public string EAN { get; set; }
        public string UPC { get; set; }
        public byte NumberVariant { get; set; }
        public short Year { get; set; }
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
        public IList<Image> Images { get; set; } = new List<Image>();
        public IList<Price> Prices { get; set; } = new List<Price>();
        public IList<Instruction> Instructions { get; set; } = new List<Instruction>();

        [BsonIgnore]
        public string NumberWithVariant => $"{Number}-{NumberVariant}";
    }
}
