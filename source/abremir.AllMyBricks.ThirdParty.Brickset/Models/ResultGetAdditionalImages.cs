using System.Collections.Generic;
using System.ComponentModel;
using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetAdditionalImages)]
    public class ResultGetAdditionalImages : ResultBase
    {
        public IEnumerable<SetImage> AdditionalImages { get; set; } = [];
    }
}
