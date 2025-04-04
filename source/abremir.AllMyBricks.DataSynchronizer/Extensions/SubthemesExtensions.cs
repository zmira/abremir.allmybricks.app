using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class SubthemesExtensions
    {
        public static Subtheme ToSubtheme(this Subthemes source)
        {
            return new Subtheme
            {
                Name = source.Subtheme.Trim(),
                SetCount = (short)source.SetCount,
                YearFrom = (short)source.YearFrom,
                YearTo = (short)source.YearTo
            };
        }
    }
}
