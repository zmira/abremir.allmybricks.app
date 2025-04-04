using System;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    public class Sets
    {
        public int SetId { get; set; }
        public string Number { get; set; }
        public int NumberVariant { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Theme { get; set; }
        public string ThemeGroup { get; set; }
        public string Subtheme { get; set; }
        public string Category { get; set; }
        public bool Released { get; set; }
        public int? Pieces { get; set; }
        public int? Minifigs { get; set; }
        public SetImage Image { get; set; }
        public string BricksetUrl { get; set; }
        public SetCollection Collection { get; set; }
        public SetCollections Collections { get; set; }
        public SetLegoCom LegoCom { get; set; }
        public float Rating { get; set; }
        public int ReviewCount { get; set; }
        public string PackagingType { get; set; }
        public string Availability { get; set; }
        public int InstructionsCount { get; set; }
        public int AdditionalImageCount { get; set; }
        public SetAgeRange AgeRange { get; set; }
        public SetDimensions Dimensions { get; set; }
        public SetBarcodes Barcode { get; set; }
        public SetExtendedData ExtendedData { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
