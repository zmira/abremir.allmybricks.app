using System.Collections.Generic;
using System.ComponentModel;
using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetThemes)]
    public class ResultGetThemes : ResultBase
    {
        public IEnumerable<Themes> Themes { get; set; } = new List<Themes>();
    }
}
