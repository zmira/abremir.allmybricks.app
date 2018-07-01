using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Factories;
using abremir.AllMyBricks.Onboarding.Helpers;
using abremir.AllMyBricks.Onboarding.Interfaces;
using ExpressMapper.Extensions;
using Flurl;
using Flurl.Http;
using System.Text;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class ApiKeyService : IApiKeyService
    {
        public string GetBricksetApiKey(Identification allMyBricksIdentification)
        {
            var client = new FlurlClient().Configure(settings => settings.HttpClientFactory = new HmacDelegatingHandlerHttpClientFactory());

            var apiKeyRequest = allMyBricksIdentification.Map<Identification, ApiKeyRequest>();

            apiKeyRequest.KeyOption = RandomKeyOptionGenerator.GetRandomKeyOption();

            var responseApiKeyResult = Constants.AllMyBricksOnboardingApiKeyService
                .AppendPathSegment(Constants.AllMyBricksOnboardingApiKeyServiceBricksetMethod)
                .WithClient(client)
                .PostJsonAsync(apiKeyRequest)
                .ReceiveString()
                .Result;

            return Jose.JWT.Decode(responseApiKeyResult, Encoding.UTF8.GetBytes(allMyBricksIdentification.RegistrationHash.ToCharArray()), (Jose.JwsAlgorithm)apiKeyRequest.KeyOption);
        }
    }
}