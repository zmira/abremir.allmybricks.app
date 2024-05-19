using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class RegistrationService(IFlurlClientCache clientCache) : IRegistrationService
    {
        private readonly IFlurlClient _flurlClient = clientCache.Get(Constants.AllMyBricksOnboardingUrlFlurlClientCacheName);

        public async Task<Identification> Register(Identification allMyBricksIdentification)
        {
            return await _flurlClient
                .Request("api", Constants.AllMyBricksOnboardingRegistrationService, Constants.AllMyBricksOnboardingRegistrationServiceRegisterMethod)
                .PostJsonAsync(allMyBricksIdentification)
                .ReceiveJson<Identification>()
                .ConfigureAwait(false);
        }

        public async Task Unregister(Identification allMyBricksIdentification)
        {
            await _flurlClient
                .Request("api", Constants.AllMyBricksOnboardingRegistrationService, Constants.AllMyBricksOnboardingRegistrationServiceUnregisterMethod)
                .PostJsonAsync(allMyBricksIdentification)
                .ConfigureAwait(false);
        }
    }
}
