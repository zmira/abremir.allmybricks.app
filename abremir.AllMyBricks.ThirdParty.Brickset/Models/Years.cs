using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlType(Namespace = Constants.BricksetApiNamespace, TypeName = Constants.TypeYears)]
    public class Years
    {
        [XmlElement(Constants.ElementTheme)]
        public string Theme { get; set; }
        [XmlElement(Constants.ElementYear)]
        public string Year { get; set; }
        [XmlElement(Constants.ElementSetCount)]
        public int SetCount { get; set; }
    }
}
