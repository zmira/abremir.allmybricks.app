using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace abremir.AllMyBricks.Data.Models
{
    [DebuggerStepThrough]
    public class Set
    {
        public long SetId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Ean { get; set; }
        public string Upc { get; set; }
        public Theme Theme { get; set; }
        public ThemeGroup ThemeGroup { get; set; }
        public Subtheme Subtheme { get; set; }
        public PackagingType PackagingType { get; set; }
        public Category Category { get; set; }
        public byte NumberVariant { get; set; }
        public short? Year { get; set; }
        public short? Pieces { get; set; }
        public short? Minifigs { get; set; }
        public string BricksetUrl { get; set; }
        public bool Released { get; set; }
        public short OwnedByTotal { get; set; }
        public short WantedByTotal { get; set; }
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

        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public IList<Image> Images { get; set; } = new List<Image>();
        public IList<Price> Prices { get; set; } = new List<Price>();
        public IList<Review> Reviews { get; set; } = new List<Review>();
        public IList<Instruction> Instructions { get; set; } = new List<Instruction>();

        public string NumberWithVariant => $"{Number}-{NumberVariant}";
    }
}