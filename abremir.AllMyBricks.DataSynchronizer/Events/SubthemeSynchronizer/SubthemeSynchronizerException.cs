using System;

namespace abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer
{
    public class SubthemeSynchronizerException
    {
        public string Theme { get; set; }
        public Exception Exception { get; set; }
    }
}