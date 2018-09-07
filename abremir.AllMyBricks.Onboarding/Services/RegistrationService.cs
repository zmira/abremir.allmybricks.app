using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Interfaces;
using Flurl;
using Flurl.Http;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly string _allMyBricksOnboardingRegistrationServiceUrl;

        public RegistrationService(string allMyBricksOnboardingUrl)
        {
            _allMyBricksOnboardingRegistrationServiceUrl = $"{allMyBricksOnboardingUrl}{Constants.AllMyBricksOnboardingRegistrationService}";
        }

        public Identification Register(Identification allMyBricksIdentification)
        {
            return _allMyBricksOnboardingRegistrationServiceUrl
                .AppendPathSegment(Constants.AllMyBricksOnboardingRegistrationServiceRegisterMethod)
                .PostJsonAsync(allMyBricksIdentification)
                .ReceiveJson<Identification>()
                .Result;
        }

        public void Unregister(Identification allMyBricksIdentification)
        {
            _allMyBricksOnboardingRegistrationServiceUrl
                .AppendPathSegment(Constants.AllMyBricksOnboardingRegistrationServiceUnregisterMethod)
                .PostJsonAsync(allMyBricksIdentification);
        }
    }
}
