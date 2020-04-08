using System.Collections.Generic;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    public class SetExtendedData
    {
        public string Notes { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
