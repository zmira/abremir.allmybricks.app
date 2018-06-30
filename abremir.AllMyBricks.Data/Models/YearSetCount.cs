using Realms;

namespace abremir.AllMyBricks.Data.Models
{
    public class YearSetCount : RealmObject
    {
        [Indexed]
        public short Year { get; set; }

        public short SetCount { get; set; }
    }
}