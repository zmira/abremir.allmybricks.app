using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Enumerations
{
    [Flags]
    public enum LogDestinations
    {
        None = 0,
        Console = 1 << 0,
        File = 1 << 1
    }
}
