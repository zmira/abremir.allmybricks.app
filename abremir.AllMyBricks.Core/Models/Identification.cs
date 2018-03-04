using abremir.AllMyBricks.Core.Extensions;
using System;

namespace abremir.AllMyBricks.Core.Models
{
    public class Identification
    {
        public Device DeviceIdentification { get; set; }
        public string RegistrationHash { get; set; }
        public DateTimeOffset? RegistrationTimestamp { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var model = obj as Identification;

            if (model == null)
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