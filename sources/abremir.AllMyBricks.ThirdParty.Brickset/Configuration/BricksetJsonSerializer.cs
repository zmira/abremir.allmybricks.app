using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http.Configuration;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Configuration
{
    internal static class BricksetJsonSerializer
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
                    options.Converters.Add(new JsonStringEnumConverter());
                    _defaultJsonSerializer = new DefaultJsonSerializer(options);
                }

                return _defaultJsonSerializer;
            }
        }
    }
}
