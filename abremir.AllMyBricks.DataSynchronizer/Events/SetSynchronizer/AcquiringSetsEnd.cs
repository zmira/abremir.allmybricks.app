namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class AcquiringSetsEnd
    {
        public string Theme { get; set; }
        public string Subtheme { get; set; }
        public int Count { get; set; }
        public int? Year { get; set; }
    }
}