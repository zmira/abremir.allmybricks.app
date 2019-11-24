using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly string _allMyBricksOnboardingRegistrationServiceUrl;

        public RegistrationService(string allMyBricksOnboardingUrl)
        {
            _allMyBricksOnboardingRegistrationServiceUrl = $"{allMyBricksOnboardingUrl}api/{Constants.AllMyBricksOnboardingRegistrationService}";
        }

        public async Task<Identification> Register(Identification allMyBricksIdentification)
        {
            return await _allMyBricksOnboardingRegistrationServiceUrl
                .AppendPathSegment(Constants.AllMyBricksOnboardingRegistrationServiceRegisterMethod)
                .PostJsonAsync(allMyBricksIdentification)
                .ReceiveJson<Identification>();
        }

        public async Task Unregister(Identification allMyBricksIdentification)
        {
            await _allMyBricksOnboardingRegistrationServiceUrl
                .AppendPathSegment(Constants.AllMyBricksOnboardingRegistrationServiceUnregisterMethod)
                .PostJsonAsync(allMyBricksIdentification);
        }
    }
}
