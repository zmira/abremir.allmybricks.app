using abremir.AllMyBricks.Data.Enumerations;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Models
{
    public class BricksetUser
    {
        public string BricksetUsername { get; set; }
        public BricksetUserTypeEnum UserType { get; set; }

        public IList<BricksetUserSet> Sets { get; set; } = new List<BricksetUserSet>();
    }
}
