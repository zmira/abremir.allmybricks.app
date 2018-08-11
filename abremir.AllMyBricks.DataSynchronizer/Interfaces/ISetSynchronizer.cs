using abremir.AllMyBricks.Data.Models;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISetSynchronizer
    {
        IEnumerable<Set> Synchronize(string apiKey, Theme theme, Subtheme subtheme);
        IEnumerable<Set> Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp);
        Set Synchronize(string apiKey, long setId);
    }
}