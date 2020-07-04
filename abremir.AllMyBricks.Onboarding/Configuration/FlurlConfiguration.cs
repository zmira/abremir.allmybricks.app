using Flurl.Http;
using Newtonsoft.Json;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class FlurlConfiguration
    {
        public static void Configure()
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffK"
            };

            FlurlHttp.Configure(configuration => configuration.JsonSerializer = new Flurl.Http.Configuration.NewtonsoftJsonSerializer(settings));
        }
    }
}
