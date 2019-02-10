using System;

namespace abremir.AllMyBricks.Device.Models
{
    public class FileSize
    {
        public long Bytes { get; private set; }
        public double Kilobytes => (double)Bytes / 1024;
        public double Megabytes => Kilobytes / 1024;
        public double Gigabytes => Megabytes / 1024;
        public double Terabytes => Gigabytes / 1024;

        public FileSize()
        {
            Bytes = 0;
        }

        public FileSize(long fileInfoLength)
        {
            Bytes = fileInfoLength;
        }

        public override string ToString()
        {
            if(Terabytes > 1)
            {
                return $"{Math.Round(Terabytes, 4)} TB";
            }

            if(Gigabytes > 1)
            {
                return $"{Math.Round(Gigabytes, 3)} GB";
            }

            if(Megabytes > 1)
            {
                return $"{Math.Round(Megabytes, 2)} MB";
            }

            if(Kilobytes > 1)
            {
                return $"{Math.Round(Kilobytes, 1)} KB";
            }

            return $"{Bytes} bytes";
        }
    }
}
