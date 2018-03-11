using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.ComponentModel;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlRoot(Namespace = Constants.BricksetApiNamespace, ElementName = Constants.RootElementString), Description(Constants.MethodCheckUserHash)]
    public class ResultCheckUserHash : ResultString
    {
    }
}
