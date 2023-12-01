using abremir.AllMyBricks.Onboarding.Shared.Models;

namespace abremir.AllMyBricks.Onboarding.Extensions
{
    public static class IdentificationExtensions
    {
        public static ApiKeyRequest ToApiKeyRequest(this Identification identification)
        {
            return new ApiKeyRequest
            {
                DeviceIdentification = identification.DeviceIdentification,
                RegistrationHash = identification.RegistrationHash,
                RegistrationTimestamp = identification.RegistrationTimestamp
            };
        }
    }
}
