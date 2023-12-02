using System;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    public class BricksetRequestException : Exception
    {
        public BricksetRequestException() { }

        public BricksetRequestException(string message) : base(message) { }

        public BricksetRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
