using abremir.AllMyBricks.Core.Security;
using abremir.AllMyBricks.Device.Interfaces;
using System;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Services
{
    public class DeviceInformationService : IDeviceInformationService
    {
        private readonly IDeviceInfo _deviceInfo;

        public DeviceInformationService(IDeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo;
        }

        public Core.Models.Device GenerateNewDeviceIdentification()
        {
            var device = new Core.Models.Device
            {
                AppId = Guid.NewGuid().ToString(),
                Manufacturer = _deviceInfo.Manufacturer,
                Model = _deviceInfo.Model,
                Version = _deviceInfo.VersionString,
                Platform = _deviceInfo.Platform,
                Idiom = _deviceInfo.Idiom,
                DeviceHashDate = DateTimeOffset.Now
            };

            device.DeviceHash = SHA256Hash.GetDeviceHash(device);

            return device;
        }
    }
}