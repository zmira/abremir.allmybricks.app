using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlType(Namespace = Constants.BricksetApiNamespace, TypeName = Constants.TypeThemes)]
    public class Themes
    {
        [XmlElement(Constants.ElementTheme)]
        public string Theme { get; set; }
        [XmlElement(Constants.ElementSetCount)]
        public int SetCount { get; set; }
        [XmlElement(Constants.ElementSubthemeCount)]
        public int SubthemeCount { get; set; }
        [XmlElement(Constants.ElementYearFrom)]
        public int YearFrom { get; set; }
        [XmlElement(Constants.ElementYearTo)]
        public int YearTo { get; set; }
    }
}
