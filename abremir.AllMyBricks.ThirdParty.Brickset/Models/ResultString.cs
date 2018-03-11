using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    public class ResultString
    {
        [XmlText(typeof(string))]
        public string Value { get; set; }
    }
}
