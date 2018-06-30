using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using Realms;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class SetRepository : ISetRepository
    {
        private readonly IRepositoryService _repositoryService;

        private IEnumerable<Set> EmptyEnumerable => new Set[] { };

        public SetRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Set AddOrUpdate(Set set)
        {
            if (set == null
                || set.SetId == 0)
            {
                return null;
            }

            var existingSet = Get(set.SetId);

            var repository = _repositoryService.GetRepository();

            repository.Write(() => repository.Add(set, existingSet != null));

            return set;
        }

        public Set Get(long setId)
        {
            if (setId == 0)
            {
                return null;
            }

            return GetQueryable().FirstOrDefault(set => set.SetId == setId);
        }

        public IEnumerable<Set> All()
        {
            return GetQueryable();
        }

        public IEnumerable<Set> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"Theme.Name ==[c] '{themeName}'");
        }

        public IEnumerable<Set> AllForSubtheme(string themeName, string subthemeName)
        {
            if(string.IsNullOrWhiteSpace(themeName) || string.IsNullOrWhiteSpace(subthemeName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"Theme.Name ==[c] '{themeName}' && Subtheme.Name ==[c] '{subthemeName}'");
        }

        public IEnumerable<Set> AllForThemeGroup(string themeGroupName)
        {
            if (string.IsNullOrWhiteSpace(themeGroupName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"ThemeGroup.Value ==[c] '{themeGroupName}'");
        }

        public IEnumerable<Set> AllForCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"Category.Value ==[c] '{categoryName}'");

        }

        public IEnumerable<Set> AllForTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"Tags.Value ==[c] '{tagName}'");
        }

        public IEnumerable<Set> AllForYear(short year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Where(set => set.Year == year);
        }

        public IEnumerable<Set> AllForPriceRange(PriceRegionEnum priceRegion, float minimumPrice, float maximumPrice)
        {
            if(minimumPrice < 0 || maximumPrice < 0)
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"Prices.RegionRaw == {(int)priceRegion} && Prices.Value >= {minimumPrice} && Prices.Value <= {maximumPrice}");
        }

        public IEnumerable<Set> SearchBy(string searchQuery)
        {
            var realmQuery = BuildRealmQueryFromSearchQuery(searchQuery);

            if (realmQuery == null)
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter(realmQuery);
        }

        private string BuildRealmQueryFromSearchQuery(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return null;
            }

            var queryList = new List<string>();

            searchQuery
                .Split(' ', '-')
                .Where(searchTerm => (searchTerm?.Trim().Length ?? 0) >= Constants.MinimumSearchQuerySize)
                .Distinct()
                .ForEach(searchTerm =>
                {
                    queryList.Add($"Number CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Name CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Ean CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Upc CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Description CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Theme.Name CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Subtheme.Name CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"ThemeGroup.Value CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Category.Value CONTAINS[c] '{searchTerm.Trim()}'");
                    queryList.Add($"Tags.Value CONTAINS[c] '{searchTerm.Trim()}'");
                });

            return queryList.Count == 0
                ? null
                : string.Join(" OR ", queryList.ToArray());
        }

        private IQueryable<Set> GetQueryable()
        {
            return _repositoryService.GetRepository().All<Set>();
        }
    }
}