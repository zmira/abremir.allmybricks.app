using abremir.AllMyBricks.ThirdParty.Brickset.Enumerations;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    public class ResultBase
    {
        public ResultStatusEnum Status { get; set; }
        public string Message { get; set; }
    }
}
