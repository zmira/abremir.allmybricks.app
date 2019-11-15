namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    public class ParameterSets : ParameterApiKeyUserHash
    {
        public string Query { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public string Subtheme { get; set; } = string.Empty;
        public string SetNumber { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Owned { get; set; } = string.Empty;
        public string Wanted { get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
        public string PageSize { get; set; } = string.Empty;
        public string PageNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
