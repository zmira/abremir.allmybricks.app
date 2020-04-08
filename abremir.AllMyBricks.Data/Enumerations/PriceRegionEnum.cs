using System.ComponentModel;

namespace abremir.AllMyBricks.Data.Enumerations
{
    public enum PriceRegionEnum
    {
        [Description("GBP")]
        UK = 1,
        [Description("USD")]
        US = 2,
        [Description("CAD")]
        CA = 3,
        [Description("EUR")]
        DE = 4
    }
}
