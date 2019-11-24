using abremir.AllMyBricks.Onboarding.Shared.Models;

namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface IDeviceInformationService
    {
        Device GenerateNewDeviceIdentification();
    }
}
