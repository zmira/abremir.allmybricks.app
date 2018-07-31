using abremir.AllMyBricks.Data.Models;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISetSynchronizer
    {
        bool Synchronize(string apiKey, Theme theme, Subtheme subtheme);
        bool Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp);
    }
}