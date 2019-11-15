using abremir.AllMyBricks.Core.Enumerations;
using abremir.AllMyBricks.Core.Extensions;

namespace abremir.AllMyBricks.Core.Models
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

            var model = obj as ApiKeyRequest;

            if (model == null)
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
