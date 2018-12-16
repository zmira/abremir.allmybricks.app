using abremir.AllMyBricks.Core.Models;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IApiKeyService
    {
        Task<string> GetBricksetApiKey(Identification allMyBricksIdentification);
    }
}