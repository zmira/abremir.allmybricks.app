using System;

namespace abremir.AllMyBricks.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime EpochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

        public static ulong TotalSecondsFromEpochStart(this DateTime date)
        {
            return Convert.ToUInt64((date - EpochStart).TotalSeconds);
        }
    }
}