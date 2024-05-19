using System;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;

namespace abremir.AllMyBricks.Onboarding.Shared.Models
{
    public class Identification
    {
        public Device DeviceIdentification { get; set; }
        public string RegistrationHash { get; set; }
        public DateTimeOffset? RegistrationTimestamp { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is not Identification model)
            {
                return false;
            }

            return model.DeviceIdentification.Equals(DeviceIdentification)
                && string.Equals(model.RegistrationHash, RegistrationHash)
                && Equals(model.RegistrationTimestamp?.ToHundredthOfSecond(), RegistrationTimestamp?.ToHundredthOfSecond());
        }

        public override int GetHashCode()
        {
            return new { DeviceIdentification, RegistrationHash, RegistrationTimestamp = RegistrationTimestamp?.ToHundredthOfSecond() }.GetHashCode();
        }
    }
}
