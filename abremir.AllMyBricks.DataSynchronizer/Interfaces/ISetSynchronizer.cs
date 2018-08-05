using abremir.AllMyBricks.Data.Models;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISetSynchronizer
    {
        IEnumerable<Set> Synchronize(string apiKey, Theme theme, Subtheme subtheme);
        bool Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp);
    }
}