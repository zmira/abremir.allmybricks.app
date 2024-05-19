using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using abremir.AllMyBricks.Onboarding.Shared.Models;

namespace abremir.AllMyBricks.Onboarding.Shared.Security
{
    public static class SHA256Hash
    {
        public static byte[] ComputeHash(Stream body)
        {
            var content = (body as MemoryStream)?.ToArray() ?? [];

            return content.Length != 0
                ? SHA256.HashData(content)
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
