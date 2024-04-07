using abremir.AllMyBricks.DataSynchronizer.Enumerations;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class AcquiringSetsStart
    {
        public SetAcquisitionType Type { get; set; }
        public GetSetsParameters Parameters { get; set; }
    }
}
