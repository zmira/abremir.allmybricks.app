using abremir.AllMyBricks.Core.Models;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IRegistrationService
    {
        Task<Identification> Register(Identification allMyBricksIdentification);
        Task Unregister(Identification allMyBricksIdentification);
    }
}