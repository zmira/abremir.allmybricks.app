using abremir.AllMyBricks.Data.Models;
using System;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Interfaces
{
    public interface ISetSynchronizer
    {
        Task Synchronize(string apiKey, Theme theme, Subtheme subtheme);
        Task Synchronize(string apiKey, DateTimeOffset previousUpdateTimestamp);
    }
}