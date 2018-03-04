using System.IO;
using System.Security.Cryptography;

namespace abremir.AllMyBricks.Core.Security
{
    public static class SHA256Hash
    {
        public static byte[] ComputeHash(Stream body)
        {
            using (var sha256 = SHA256.Create())
            {
                var content = (body as MemoryStream)?.ToArray() ?? new byte[] { };

                return content.Length != 0
                    ? sha256.ComputeHash(content)
                    : null;
            }
        }
    }
}