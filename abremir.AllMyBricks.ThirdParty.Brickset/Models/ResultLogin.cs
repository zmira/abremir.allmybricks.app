using System.ComponentModel;
using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    [Description(Constants.MethodLogin)]
    public class ResultLogin : ResultBase
    {
        public string Hash { get; set; }
    }
}
