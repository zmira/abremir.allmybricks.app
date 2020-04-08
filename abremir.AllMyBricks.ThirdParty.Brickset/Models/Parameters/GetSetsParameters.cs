using Newtonsoft.Json.Linq;
using System;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters
{
    public class GetSetsParameters : ParameterApiKeyUserHash
    {
        public long? SetId { get; set; }
        public string Query { get; set; }
        public string Theme { get; set; }
        public string Subtheme { get; set; }
        public string SetNumber { get; set; }
        public int? Year { get; set; }
        public string Tag { get; set; }
        public bool? Owned { get; set; }
        public bool? Wanted { get; set; }
        public DateTime? UpdatedSince { get; set; }
        public string OrderBy { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public bool? ExtendedData { get; set; }

        public ParameterSets ToParameterSets()
        {
            return new ParameterSets
            {
                ApiKey = ApiKey,
                UserHash = UserHash,
                Params = GetParams()
            };
        }

        private string GetParams()
        {
            dynamic @params = new JObject();

            if (SetId.HasValue)
            {
                @params.SetID = SetId.Value;
            }

            if (!string.IsNullOrWhiteSpace(Query))
            {
                @params.Query = Query.Trim();
            }

            if (!string.IsNullOrWhiteSpace(Theme))
            {
                @params.Theme = Theme.Trim();
            }

            if (!string.IsNullOrWhiteSpace(Subtheme))
            {
                @params.Subtheme = Subtheme.Trim();
            }

            if (!string.IsNullOrWhiteSpace(SetNumber))
            {
                @params.SetNumber = SetNumber.Trim();
            }

            if (Year.HasValue)
            {
                @params.Year = Year.Value.ToString();
            }

            if (!string.IsNullOrWhiteSpace(Tag))
            {
                @params.Tag = Tag.Trim();
            }

            if (Owned.HasValue)
            {
                @params.Owned = Owned.Value ? 1 : 0;
            }

            if (Wanted.HasValue)
            {
                @params.Wanted = Wanted.Value ? 1 : 0;
            }

            if (UpdatedSince.HasValue)
            {
                @params.UpdatedSince = UpdatedSince.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (!string.IsNullOrWhiteSpace(OrderBy))
            {
                @params.OrderBy = OrderBy.Trim();
            }

            if (PageSize.HasValue)
            {
                @params.PageSize = PageSize.Value;
            }

            if (PageNumber.HasValue)
            {
                @params.PageNumber = PageNumber.Value;
            }

            if (ExtendedData.HasValue)
            {
                @params.ExtendedData = ExtendedData.Value ? 1 : 0;
            }

            return @params.ToString();
        }
    }
}
