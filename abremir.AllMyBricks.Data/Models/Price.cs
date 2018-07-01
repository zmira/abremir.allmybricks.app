using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.Data.Models
{
    public class Price
    {
        public PriceRegionEnum Region { get; set; }
        public float Value { get; set; }
    }
}