using abremir.AllMyBricks.Onboarding.Shared.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace abremir.AllMyBricks.Onboarding.Shared.Security
{
    public static class SHA256Hash
    {
        public static byte[] ComputeHash(Stream body)
        {
            using var sha256 = SHA256.Create();

            var content = (body as MemoryStream)?.ToArray() ?? Array.Empty<byte>();

            return content.Length != 0
                ? sha256.ComputeHash(content)
                : null;
        }

        public static byte[] ComputeHash(string textToHash)
        {
            return ComputeHash(
                new MemoryStream(
                    Encoding.ASCII.GetBytes(
                        textToHash ?? string.Empty
                    )
                )
            );
        }

        public static string GetDeviceHash(Device device)
        {
            return Convert
                .ToBase64String(
                    ComputeHash(
                        $"{device.AppId}_{device.Manufacturer}_{device.Model}_{device.Version}_{device.Platform}_{device.Idiom}"
                    )
                );
        }
    }
}
