using Newtonsoft.Json.Linq;

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
            dynamic @params = new JObject();

            @params.Own = Own ? 1 : 0;
            @params.Want = Want ? 1 : 0;
            @params.QtyOwned = QtyOwned;
            @params.Notes = Notes?.Substring(0, 200).Trim();
            @params.Rating = Rating;

            return @params.ToString();
        }
    }
}
