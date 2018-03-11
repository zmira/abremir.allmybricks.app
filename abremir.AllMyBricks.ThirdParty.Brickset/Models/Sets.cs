using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlType(Namespace = Constants.BricksetApiNamespace, TypeName = Constants.TypeSets)]
    public class Sets
    {
        [XmlElement(Constants.ElementSetId)]
        public int SetId { get; set; }
        [XmlElement(Constants.ElementNumber)]
        public string Number { get; set; }
        [XmlElement(Constants.ElementNumberVariant)]
        public int NumberVariant { get; set; }
        [XmlElement(Constants.ElementName)]
        public string Name { get; set; }
        [XmlElement(Constants.ElementYear)]
        public string Year { get; set; }
        [XmlElement(Constants.ElementTheme)]
        public string Theme { get; set; }
        [XmlElement(Constants.ElementThemeGroup)]
        public string ThemeGroup { get; set; }
        [XmlElement(Constants.ElementSubtheme)]
        public string Subtheme { get; set; }
        [XmlElement(Constants.ElementPieces)]
        public string Pieces { get; set; }
        [XmlElement(Constants.ElementMinifigs)]
        public string Minifigs { get; set; }
        [XmlElement(Constants.ElementImage)]
        public bool Image { get; set; }
        [XmlElement(Constants.ElementImageFilename)]
        public string ImageFilename { get; set; }
        [XmlElement(Constants.ElementThumbnailUrl)]
        public string ThumbnailUrl { get; set; }
        [XmlElement(Constants.ElementLargeThumbnailUrl)]
        public string LargeThumbnailUrl { get; set; }
        [XmlElement(Constants.ElementImageUrl)]
        public string ImageUrl { get; set; }
        [XmlElement(Constants.ElementBricksetUrl)]
        public string BricksetUrl { get; set; }
        [XmlElement(Constants.ElementReleased)]
        public bool Released { get; set; }
        [XmlElement(Constants.ElementOwned)]
        public bool Owned { get; set; }
        [XmlElement(Constants.ElementWanted)]
        public bool Wanted { get; set; }
        [XmlElement(Constants.ElementQtyOwned)]
        public int QtyOwned { get; set; }
        [XmlElement(Constants.ElementUserNotes)]
        public string UserNotes { get; set; }
        [XmlElement(Constants.ElementAcmDataCount)]
        public int AcmDataCount { get; set; }
        [XmlElement(Constants.ElementOwnedByTotal)]
        public int OwnedByTotal { get; set; }
        [XmlElement(Constants.ElementWantedByTotal)]
        public int WantedByTotal { get; set; }
        [XmlElement(Constants.ElementUkRetailPrice)]
        public string UkRetailPrice { get; set; }
        [XmlElement(Constants.ElementUsRetailPrice)]
        public string UsRetailPrice { get; set; }
        [XmlElement(Constants.ElementCaRetailPrice)]
        public string CaRetailPrice { get; set; }
        [XmlElement(Constants.ElementEuRetailPrice)]
        public string EuRetailPrice { get; set; }
        [XmlElement(Constants.ElementUsDateAddedToSah)]
        public string UsDateAddedToSah { get; set; }
        [XmlElement(Constants.ElementUsDateRemovedFromSah)]
        public string UsDateRemovedFromSah { get; set; }
        [XmlElement(Constants.ElementRating)]
        public decimal Rating { get; set; }
        [XmlElement(Constants.ElementReviewCount)]
        public int ReviewCount { get; set; }
        [XmlElement(Constants.ElementPackagingType)]
        public string PackagingType { get; set; }
        [XmlElement(Constants.ElementAvailability)]
        public string Availability { get; set; }
        [XmlElement(Constants.ElementInstructionsCount)]
        public int InstructionsCount { get; set; }
        [XmlElement(Constants.ElementAdditionalImageCount)]
        public int AdditionalImageCount { get; set; }
        [XmlElement(Constants.ElementAgeMin)]
        public string AgeMin { get; set; }
        [XmlElement(Constants.ElementAgeMax)]
        public string AgeMax { get; set; }
        [XmlElement(Constants.ElementHeight)]
        public string Height { get; set; }
        [XmlElement(Constants.ElementWidth)]
        public string Width { get; set; }
        [XmlElement(Constants.ElementDepth)]
        public string Depth { get; set; }
        [XmlElement(Constants.ElementWeight)]
        public string Weight { get; set; }
        [XmlElement(Constants.ElementCategory)]
        public string Category { get; set; }
        [XmlElement(Constants.ElementNotes)]
        public string Notes { get; set; }
        [XmlElement(Constants.ElementUserRating)]
        public string UserRating { get; set; }
        [XmlElement(Constants.ElementTags)]
        public string Tags { get; set; }
        [XmlElement(Constants.ElementEan)]
        public string Ean { get; set; }
        [XmlElement(Constants.ElementUpc)]
        public string Upc { get; set; }
        [XmlElement(Constants.ElementDescription)]
        public string Description { get; set; }
        [XmlElement(Constants.ElementLastUpdated)]
        public DateTime LastUpdated { get; set; }
    }
}