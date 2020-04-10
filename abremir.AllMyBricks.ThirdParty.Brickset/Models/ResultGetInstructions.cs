using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetInstructions)]
    public class ResultGetInstructions : ResultBase
    {
        public IEnumerable<Instructions> Instructions { get; set; } = new List<Instructions>();
    }
}
