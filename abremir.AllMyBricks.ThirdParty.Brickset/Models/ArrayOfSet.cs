using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [XmlRoot(Namespace = Constants.BricksetApiNamespace, ElementName = Constants.RootElementArrayOfSets), Description(Constants.MethodGetSet)]
    public class ArrayOfSet : List<Sets>
    {
    }
}