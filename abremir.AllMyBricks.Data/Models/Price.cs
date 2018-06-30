using abremir.AllMyBricks.Data.Enumerations;
using Realms;

namespace abremir.AllMyBricks.Data.Models
{
    public class Price : RealmObject
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