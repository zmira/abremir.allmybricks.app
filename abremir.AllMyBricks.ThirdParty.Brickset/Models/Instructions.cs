using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlType(Namespace = Constants.BricksetApiNamespace, TypeName = Constants.TypeInstructions)]
    public class Instructions
    {
        [XmlElement(Constants.ElementUrl)]
        public string Url { get; set; }
        [XmlElement(Constants.ElementDescription)]
        public string Description { get; set; }
    }
}
