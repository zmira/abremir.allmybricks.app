using abremir.AllMyBricks.Data.Models;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISetSynchronizer
    {
        void Synchronize(string apiKey, Theme theme, Subtheme subtheme);
        void Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp);
    }
}