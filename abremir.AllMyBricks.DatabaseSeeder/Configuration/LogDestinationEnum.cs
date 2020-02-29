using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    [Flags]
    public enum LogDestinationEnum
    {
        Console = 1 << 0,
        File = 1 << 1
    }
}
