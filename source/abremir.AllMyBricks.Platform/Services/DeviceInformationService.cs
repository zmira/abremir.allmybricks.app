using System;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Devices;

namespace abremir.AllMyBricks.Platform.Services
{
    public class DeviceInformationService(IDeviceInfo deviceInfo) : IDeviceInformationService
    {
        private readonly IDeviceInfo _deviceInfo = deviceInfo;

        public Device GenerateNewDeviceIdentification()
        {
            var device = new Device
            {
                AppId = Guid.NewGuid().ToString(),
                Manufacturer = _deviceInfo.Manufacturer,
                Model = _deviceInfo.Model,
                Version = _deviceInfo.VersionString,
                Platform = _deviceInfo.Platform.ToString(),
                Idiom = _deviceInfo.Idiom.ToString(),
                DeviceHashDate = DateTimeOffset.Now
            };

            device.DeviceHash = SHA256Hash.GetDeviceHash(device);

            return device;
        }
    }
}
