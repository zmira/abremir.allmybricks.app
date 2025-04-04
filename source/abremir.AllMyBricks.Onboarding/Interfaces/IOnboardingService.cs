using System.Threading.Tasks;

namespace abremir.AllMyBricks.Onboarding.Interfaces
{
    public interface IOnboardingService
    {
        Task<string> GetBricksetApiKey();
    }
}
