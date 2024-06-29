using System.Text.Json;

namespace abremir.AllMyBricks.Onboarding.Shared.Configuration
{
    public static class Constants
    {
        public const long TicksPerHundredthOfSecond = 100000;
        public const string HmacAuthenticationScheme = "amx";
        public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    }
}
