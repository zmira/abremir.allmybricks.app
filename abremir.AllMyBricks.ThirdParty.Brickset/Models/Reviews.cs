using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlType(Namespace = Constants.BricksetApiNamespace, TypeName = Constants.TypeReviews)]
    public class Reviews
    {
        [XmlElement(Constants.ElementAuthor)]
        public string Author { get; set; }
        [XmlElement(Constants.ElementDatePosted)]
        public DateTime DatePosted { get; set; }
        [XmlElement(Constants.ElementOverallRating)]
        public int OverallRating { get; set; }
        [XmlElement(Constants.ElementParts)]
        public int Parts { get; set; }
        [XmlElement(Constants.ElementBuildingExperience)]
        public int BuildingExperience { get; set; }
        [XmlElement(Constants.ElementPlayability)]
        public int Playability { get; set; }
        [XmlElement(Constants.ElementValueForMoney)]
        public int ValueForMoney { get; set; }
        [XmlElement(Constants.ElementTitle)]
        public string Title { get; set; }
        [XmlElement(Constants.ElementReview)]
        public string Review { get; set; }
        [XmlElement(Constants.ElementHtml)]
        public bool Html { get; set; }
    }
}
