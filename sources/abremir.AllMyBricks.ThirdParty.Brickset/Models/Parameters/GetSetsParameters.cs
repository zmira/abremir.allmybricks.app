using System;
using Newtonsoft.Json.Linq;

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
                @params.setID = SetId.Value;
            }

            if (!string.IsNullOrWhiteSpace(Query))
            {
                @params.query = Query.Trim();
            }

            if (!string.IsNullOrWhiteSpace(Theme))
            {
                @params.theme = Theme.Trim();
            }

            if (!string.IsNullOrWhiteSpace(Subtheme))
            {
                @params.subtheme = Subtheme.Trim();
            }

            if (!string.IsNullOrWhiteSpace(SetNumber))
            {
                @params.setNumber = SetNumber.Trim();
            }

            if (Year.HasValue)
            {
                @params.year = Year.Value.ToString();
            }

            if (!string.IsNullOrWhiteSpace(Tag))
            {
                @params.tag = Tag.Trim();
            }

            if (Owned.HasValue)
            {
                @params.owned = Owned.Value ? 1 : 0;
            }

            if (Wanted.HasValue)
            {
                @params.wanted = Wanted.Value ? 1 : 0;
            }

            if (UpdatedSince.HasValue)
            {
                @params.updatedSince = UpdatedSince.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (!string.IsNullOrWhiteSpace(OrderBy))
            {
                @params.orderBy = OrderBy.Trim();
            }

            if (PageSize.HasValue)
            {
                @params.pageSize = PageSize.Value;
            }

            if (PageNumber.HasValue)
            {
                @params.pageNumber = PageNumber.Value;
            }

            if (ExtendedData.HasValue)
            {
                @params.extendedData = ExtendedData.Value ? 1 : 0;
            }

            return @params.ToString();
        }
    }
}
