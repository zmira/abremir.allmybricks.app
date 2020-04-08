using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using System.Collections.Generic;
using System.ComponentModel;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodGetThemes)]
    public class ResultGetThemes : ResultBase
    {
        public IEnumerable<Themes> Themes { get; set; } = new List<Themes>();
    }
}
