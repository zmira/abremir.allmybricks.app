using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlType(Namespace = Constants.BricksetApiNamespace, TypeName = Constants.TypeAdditionalImages)]
    public class AdditionalImages
    {
        [XmlElement(Constants.ElementThumbnailUrl)]
        public string ThumbnailUrl { get; set; }
        [XmlElement(Constants.ElementLargeThumbnailUrl)]
        public string LargeThumbnailUrl { get; set; }
        [XmlElement(Constants.ElementImageUrl)]
        public string ImageUrl { get; set; }
    }
}
