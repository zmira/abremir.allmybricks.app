using abremir.AllMyBricks.Data.Enumerations;
using Realms;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    internal class Price : RealmObject
    {
        [Indexed]
        public byte RegionRaw { get; set; }

        public float Value { get; set; }

        public PriceRegionEnum Region
        {
            get => (PriceRegionEnum)RegionRaw;
            set => RegionRaw = (byte)value;
        }
    }
}