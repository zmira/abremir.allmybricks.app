namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IDeviceInformationService
    {
        Onboarding.Shared.Models.Device GenerateNewDeviceIdentification();
    }
}
