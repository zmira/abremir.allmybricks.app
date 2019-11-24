using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using abremir.AllMyBricks.Platform.Interfaces;
using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class DeviceInformationService : IDeviceInformationService
    {
        public Device GenerateNewDeviceIdentification()
        {
            var device = new Device
            {
                AppId = Guid.NewGuid().ToString(),
                Manufacturer = Environment.MachineName,
                Model = Environment.UserName,
                Version = Environment.OSVersion.VersionString,
                Platform = Environment.OSVersion.Platform.ToString(),
                Idiom = string.Empty,
                DeviceHashDate = DateTimeOffset.Now
            };

            device.DeviceHash = SHA256Hash.GetDeviceHash(device);

            return device;
        }
    }
}
