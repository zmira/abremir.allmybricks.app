using System.Text;
using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Extensions;
using abremir.AllMyBricks.Onboarding.Helpers;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using Flurl.Http;
using Flurl.Http.Configuration;
using Jose;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IFlurlClient _flurlClient;

        public ApiKeyService(IFlurlClientCache clientCache)
        {
            _flurlClient = clientCache.Get(Constants.AllMyBricksOnboardingHmacUrlFlurlClientCacheName);
        }

        public async Task<string> GetBricksetApiKey(Identification allMyBricksIdentification)
        {
            var apiKeyRequest = allMyBricksIdentification.ToApiKeyRequest();

            apiKeyRequest.KeyOption = RandomKeyOptionGenerator.GetRandomKeyOption();

            var responseApiKeyResult = await _flurlClient
                .Request("api", Constants.AllMyBricksOnboardingApiKeyService, Constants.AllMyBricksOnboardingApiKeyServiceBricksetMethod)
                .PostJsonAsync(apiKeyRequest)
                .ReceiveString()
                .ConfigureAwait(false);

            return JWT.Decode(responseApiKeyResult, Encoding.UTF8.GetBytes(allMyBricksIdentification.RegistrationHash.ToCharArray()), (JwsAlgorithm)apiKeyRequest.KeyOption);
        }
    }
}
