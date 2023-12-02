namespace abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer
{
    public class SynchronizingSetStart
    {
        public string Theme { get; set; }
        public string Subtheme { get; set; }
        public string Number { get; set; }
        public int NumberVariant { get; set; }
        public string Name { get; set; }
        public int? Year { get; set; }

        public string IdentifierShort => $"{Number}-{NumberVariant} {Name}";
        public string IdentifierLong => $"{IdentifierShort} ({Theme}-{Subtheme})";
    }
}
