using abremir.AllMyBricks.Onboarding.Shared.Security;
using Flurl.Http.Configuration;
using System.Net.Http;

namespace abremir.AllMyBricks.Onboarding.Factories
{
    public class HmacDelegatingHandlerHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            var handler = base.CreateMessageHandler();

            return new HmacDelegatingHandler(handler);
        }
    }
}
