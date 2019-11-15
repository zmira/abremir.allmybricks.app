using System.ComponentModel;

namespace abremir.AllMyBricks.Data.Enumerations
{
    public enum RatingItemEnum
    {
        [Description("Overall Rating")]
        Overall = 1,

        [Description("Parts")]
        Parts = 2,

        [Description("Building Experience")]
        BuildingExperience = 3,

        [Description("Playability")]
        Playability = 4,

        [Description("Value for Money")]
        ValueForMoney = 5
    }
}
