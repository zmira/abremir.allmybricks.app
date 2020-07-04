using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    [Flags]
    public enum LogDestinationEnum
    {
        None = 0,
        Console = 1 << 0,
        File = 1 << 1
    }
}
