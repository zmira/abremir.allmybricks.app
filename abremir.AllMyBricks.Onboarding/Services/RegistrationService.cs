using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Interfaces;
using Flurl;
using Flurl.Http;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class RegistrationService : IRegistrationService
    {
        public Identification Register(Identification allMyBricksIdentification)
        {
            return Constants.AllMyBricksOnboardingRegistrationService
                .AppendPathSegment(Constants.AllMyBricksOnboardingRegistrationServiceRegisterMethod)
                .PostJsonAsync(allMyBricksIdentification)
                .ReceiveJson<Identification>()
                .Result;
        }

        public void Unregister(Identification allMyBricksIdentification)
        {
            Constants.AllMyBricksOnboardingRegistrationService
                .AppendPathSegment(Constants.AllMyBricksOnboardingRegistrationServiceUnregisterMethod)
                .PostJsonAsync(allMyBricksIdentification);
        }
    }
}
