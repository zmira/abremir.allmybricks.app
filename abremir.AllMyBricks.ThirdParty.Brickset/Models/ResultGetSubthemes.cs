using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetSubthemes)]
    public class ResultGetSubthemes : ResultBase
    {
        public IEnumerable<Subthemes> Subthemes { get; set; } = new List<Subthemes>();
    }
}
