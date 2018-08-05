using Realms;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    public class Set : RealmObject
    {
        [PrimaryKey]
        public long SetId { get; set; }

        [Indexed]
        public string Number { get; set; }

        [Indexed]
        public string Name { get; set; }

        [Indexed]
        public string Description { get; set; }

        [Indexed]
        public string Ean { get; set; }

        [Indexed]
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
        public short? Height { get; set; }
        public short? Width { get; set; }
        public short? Depth { get; set; }
        public short? Weight { get; set; }
        public string Notes { get; set; }
        public string UserRating { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public IList<Tag> Tags { get; }
        public IList<Image> Images { get; }
        public IList<Price> Prices { get; }
        public IList<Review> Reviews { get; }
        public IList<Instruction> Instructions { get; }
    }
}