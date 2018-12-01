using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SetSynchronizerStart : IDataSynchronizerEvent
    {
        public bool ForSubtheme { get; set; }
    }
}