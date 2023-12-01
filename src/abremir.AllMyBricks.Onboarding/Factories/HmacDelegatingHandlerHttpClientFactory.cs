using System.Net.Http;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using Flurl.Http.Configuration;

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
