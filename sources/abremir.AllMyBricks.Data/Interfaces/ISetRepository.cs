using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface ISetRepository
    {
        Task<Set> AddOrUpdate(Set set);
        Task<Set> Get(long setId);
        Task<IEnumerable<Set>> All();
        Task<IEnumerable<Set>> AllForTheme(string themeName);
        Task<IEnumerable<Set>> AllForSubtheme(string themeName, string subthemeName);
        Task<IEnumerable<Set>> AllForThemeGroup(string themeGroupName);
        Task<IEnumerable<Set>> AllForCategory(string categoryName);
        Task<IEnumerable<Set>> AllForTag(string tagName);
        Task<IEnumerable<Set>> AllForYear(short year);
        Task<IEnumerable<Set>> AllForPriceRange(PriceRegion priceRegion, float minimumPrice, float maximumPrice);
        Task<IEnumerable<Set>> SearchBy(string searchQuery);
        Task<int> Count();
        Task<int> DeleteMany(List<long> setIds);
        Task<IEnumerable<Set>> Find(Expression<Func<Set, bool>> predicate);
    }
}
