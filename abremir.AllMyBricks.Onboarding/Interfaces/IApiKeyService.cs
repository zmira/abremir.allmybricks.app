using abremir.AllMyBricks.Core.Models;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IApiKeyService
    {
        string GetBricksetApiKey(Identification allMyBricksIdentification);
    }
}