using abremir.AllMyBricks.Core.Security;
using abremir.AllMyBricks.Device.Interfaces;
using System;

namespace abremir.AllMyBricks.DatabaseFeeder.Implementations
{
    public class DeviceInformationService : IDeviceInformationService
    {
        public Core.Models.Device GenerateNewDeviceIdentification()
        {
            var device = new Core.Models.Device
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