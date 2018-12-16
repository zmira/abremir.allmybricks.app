using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Extensions;
using abremir.AllMyBricks.Onboarding.Factories;
using abremir.AllMyBricks.Onboarding.Helpers;
using abremir.AllMyBricks.Onboarding.Interfaces;
using Flurl;
using Flurl.Http;
using Jose;
using System.Text;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly string _allMyBricksOnboardingApiKeyServiceUrl;

        public ApiKeyService(string allMyBricksOnboardingUrl)
        {
            _allMyBricksOnboardingApiKeyServiceUrl = $"{allMyBricksOnboardingUrl}api/{Constants.AllMyBricksOnboardingApiKeyService}";
        }

        public async Task<string> GetBricksetApiKey(Identification allMyBricksIdentification)
        {
            var client = new FlurlClient().Configure(settings => settings.HttpClientFactory = new HmacDelegatingHandlerHttpClientFactory());

            var apiKeyRequest = allMyBricksIdentification.ToApiKeyRequest();

            apiKeyRequest.KeyOption = RandomKeyOptionGenerator.GetRandomKeyOption();

            var responseApiKeyResult = await _allMyBricksOnboardingApiKeyServiceUrl
                .AppendPathSegment(Constants.AllMyBricksOnboardingApiKeyServiceBricksetMethod)
                .WithClient(client)
                .PostJsonAsync(apiKeyRequest)
                .ReceiveString();

            return JWT.Decode(responseApiKeyResult, Encoding.UTF8.GetBytes(allMyBricksIdentification.RegistrationHash.ToCharArray()), (JwsAlgorithm)apiKeyRequest.KeyOption);
        }
    }
}