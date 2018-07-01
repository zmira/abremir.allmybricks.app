using Realms;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    internal class YearSetCount : RealmObject
    {
        [Indexed]
        public short Year { get; set; }

        public short SetCount { get; set; }
    }
}