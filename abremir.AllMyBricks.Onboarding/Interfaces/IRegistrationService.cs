using abremir.AllMyBricks.Core.Models;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IRegistrationService
    {
        Identification Register(Identification allMyBricksIdentification);
        void Unregister(Identification allMyBricksIdentification);
    }
}