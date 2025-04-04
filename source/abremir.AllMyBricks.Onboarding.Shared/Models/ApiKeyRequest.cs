using abremir.AllMyBricks.Onboarding.Shared.Enumerations;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;

namespace abremir.AllMyBricks.Onboarding.Shared.Models
{
    public class ApiKeyRequest : Identification
    {
        public AlgorithmType KeyOption { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is not ApiKeyRequest model)
            {
                return false;
            }

            return model.KeyOption.Equals(KeyOption)
                && base.Equals(model);
        }

        public override int GetHashCode()
        {
            return new { DeviceIdentification, RegistrationHash, RegistrationTimestamp = RegistrationTimestamp?.ToHundredthOfSecond(), KeyOption }.GetHashCode();
        }
    }
}
