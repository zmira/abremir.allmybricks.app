namespace abremir.AllMyBricks.ThirdParty.Brickset.Configuration
{
    public static class Constants
    {
        public const string BricksetApiNamespace = "https://brickset.com/api/";
        public static readonly string BricksetApiUrl = $"{BricksetApiNamespace}v3.asmx";

        public const string MethodCheckKey = "checkKey";
        public const string MethodLogin = "login";
        public const string MethodCheckUserHash = "checkUserHash";
        public const string MethodGetThemes = "getThemes";
        public const string MethodGetSubthemes = "getSubthemes";
        public const string MethodGetYears = "getYears";
        public const string MethodGetSets = "getSets";
        public const string MethodGetInstructions = "getInstructions";
        public const string MethodGetAdditionalImages = "getAdditionalImages";
        public const string MethodSetCollection = "setCollection";
    }
}
