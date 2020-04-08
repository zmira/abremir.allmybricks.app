using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetYears)]
    public class ResultGetYears : ResultBase
    {
        public IEnumerable<Years> Years { get; set; } = new List<Years>();
    }
}
