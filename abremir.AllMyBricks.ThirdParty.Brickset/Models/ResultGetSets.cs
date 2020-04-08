using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetSets)]
    public class ResultGetSets : ResultBase
    {
        public IEnumerable<Sets> Sets { get; set; } = new List<Sets>();
        public int Matches { get; set; }
    }
}
