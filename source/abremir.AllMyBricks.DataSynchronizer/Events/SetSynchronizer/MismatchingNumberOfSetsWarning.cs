namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class MismatchingNumberOfSetsWarning
    {
        public int Expected { get; set; }
        public int Actual { get; set; }
    }
}
