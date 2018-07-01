using abremir.AllMyBricks.Data.Enumerations;
using Realms;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    internal class RatingItem : RealmObject
    {
        [Indexed]
        public byte TypeRaw { get; set; }

        [Indexed]
        public byte Value { get; set; }

        public RatingItemEnum Type
        {
            get => (RatingItemEnum)TypeRaw;
            set => TypeRaw = (byte)value;
        }
    }
}