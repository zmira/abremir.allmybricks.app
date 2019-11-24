using abremir.AllMyBricks.Onboarding.Shared.Models;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IRegistrationService
    {
        Task<Identification> Register(Identification allMyBricksIdentification);
        Task Unregister(Identification allMyBricksIdentification);
    }
}
