using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetAdditionalImages)]
    public class ResultGetAdditionalImages : ResultBase
    {
        public IEnumerable<SetImage> AdditionalImages { get; set; } = new List<SetImage>();
    }
}
