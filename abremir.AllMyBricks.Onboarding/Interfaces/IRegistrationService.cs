using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Shared.Models;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IRegistrationService
    {
        Task<Identification> Register(Identification allMyBricksIdentification);
        Task Unregister(Identification allMyBricksIdentification);
    }
}
