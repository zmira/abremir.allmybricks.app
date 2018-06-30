using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface ISetRepository
    {
        Set AddOrUpdate(Set set);
        Set Get(long setId);
        IEnumerable<Set> All();
        IEnumerable<Set> AllForTheme(string themeName);
        IEnumerable<Set> AllForSubtheme(string themeName, string subthemeName);
        IEnumerable<Set> AllForThemeGroup(string themeGroupName);
        IEnumerable<Set> AllForCategory(string categoryName);
        IEnumerable<Set> AllForTag(string tagName);
        IEnumerable<Set> AllForYear(short year);
        IEnumerable<Set> AllForPriceRange(PriceRegionEnum priceRegion, float minimumPrice, float maximumPrice);
        IEnumerable<Set> SearchBy(string searchQuery);
    }
}