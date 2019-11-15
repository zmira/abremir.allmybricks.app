using abremir.AllMyBricks.Core.Extensions;
using System;

namespace abremir.AllMyBricks.Core.Models
{
    public class Device
    {
        public string AppId { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public string DeviceHash { get; set; }
        public DateTimeOffset? DeviceHashDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var device = obj as Device;

            if (device == null)
            {
                return false;
            }

            return string.Equals(device.AppId, AppId)
                && string.Equals(device.Manufacturer, Manufacturer)
                && string.Equals(device.Model, Model)
                && string.Equals(device.Version, Version)
                && string.Equals(device.Platform, Platform)
                && string.Equals(device.Idiom, Idiom)
                && string.Equals(device.DeviceHash, DeviceHash)
                && Equals(device.DeviceHashDate?.ToHundredthOfSecond(), DeviceHashDate?.ToHundredthOfSecond());
        }

        public override int GetHashCode()
        {
            return new { AppId, Manufacturer, Model, Version, Platform, Idiom, DeviceHash, DeviceHashDate = DeviceHashDate?.ToHundredthOfSecond() }.GetHashCode();
        }
    }
}
