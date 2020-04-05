using abremir.AllMyBricks.Onboarding.Shared.Enumerations;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;

namespace abremir.AllMyBricks.Onboarding.Shared.Models
{
    public class ApiKeyRequest : Identification
    {
        public AlgorithmTypeEnum KeyOption { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is ApiKeyRequest model))
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
