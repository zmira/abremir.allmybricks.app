using System.Collections.Generic;
using System.ComponentModel;
using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetSubthemes)]
    public class ResultGetSubthemes : ResultBase
    {
        public IEnumerable<Subthemes> Subthemes { get; set; } = [];
    }
}
