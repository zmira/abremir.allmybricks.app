using System.Text.Json;
using abremir.AllMyBricks.Onboarding.Helpers;
using Flurl.Http.Configuration;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    internal static class OnboardingJsonSerializer
    {
        private static DefaultJsonSerializer _defaultJsonSerializer;
        public static DefaultJsonSerializer JsonSerializer
        {
            get
            {
                if (_defaultJsonSerializer is null)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    options.Converters.Add(new CustomDateTimeOffsetConverter("yyyy-MM-ddTHH:mm:ss.fffffffK"));
                    _defaultJsonSerializer = new DefaultJsonSerializer(options);
                }

                return _defaultJsonSerializer;
            }
        }
    }
}
