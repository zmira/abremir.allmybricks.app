using System;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Devices;

namespace abremir.AllMyBricks.Platform.Services
{
    public class DeviceInformationService(IDeviceInfo deviceInfo) : IDeviceInformationService
    {
        public Device GenerateNewDeviceIdentification()
        {
            var device = new Device
            {
                AppId = Guid.NewGuid().ToString(),
                Manufacturer = deviceInfo.Manufacturer,
                Model = deviceInfo.Model,
                Version = deviceInfo.VersionString,
                Platform = deviceInfo.Platform.ToString(),
                Idiom = deviceInfo.Idiom.ToString(),
                DeviceHashDate = DateTimeOffset.Now
            };

            device.DeviceHash = SHA256Hash.GetDeviceHash(device);

            return device;
        }
    }
}
