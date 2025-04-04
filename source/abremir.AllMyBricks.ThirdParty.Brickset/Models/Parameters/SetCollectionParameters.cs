using System.Dynamic;
using System.Text.Json;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters
{
    public class SetCollectionParameters : ParameterUserHashSetId
    {
        public bool Own { get; set; }
        public bool Want { get; set; }
        public int QtyOwned { get; set; }
        public string Notes { get; set; }
        public int Rating { get; set; }

        public ParameterSetCollection ToParameterSetCollection()
        {
            return new ParameterSetCollection
            {
                ApiKey = ApiKey,
                SetID = SetID,
                UserHash = UserHash,
                Params = GetParams()
            };
        }

        private string GetParams()
        {
            dynamic @params = new ExpandoObject();

            @params.own = Own ? 1 : 0;
            @params.want = Want ? 1 : 0;
            @params.qtyOwned = QtyOwned;
            @params.notes = Notes?[..200].Trim();
            @params.rating = Rating;

            return JsonSerializer.Serialize(@params);
        }
    }
}
