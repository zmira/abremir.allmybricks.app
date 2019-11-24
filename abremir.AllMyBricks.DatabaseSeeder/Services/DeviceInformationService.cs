using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class DeviceInformationService : IDeviceInformationService
    {
        public Onboarding.Shared.Models.Device GenerateNewDeviceIdentification()
        {
            var device = new Onboarding.Shared.Models.Device
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
