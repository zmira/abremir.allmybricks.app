using System.Collections.Generic;
using System.ComponentModel;
using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetInstructions)]
    public class ResultGetInstructions : ResultBase
    {
        public IEnumerable<Instructions> Instructions { get; set; } = new List<Instructions>();
    }
}
