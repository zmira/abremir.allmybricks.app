using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Shared.Models;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IApiKeyService
    {
        Task<string> GetBricksetApiKey(Identification allMyBricksIdentification);
    }
}
