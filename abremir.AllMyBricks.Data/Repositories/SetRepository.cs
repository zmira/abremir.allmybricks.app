using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class SetRepository : ISetRepository
    {
        private readonly IRepositoryService _repositoryService;

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

            set.TrimAllStrings();

            using (var repository = _repositoryService.GetRepository())
            {
                repository.Upsert(set);
            }

            return set;
        }

        public IEnumerable<Set> All()
        {
            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository).ToList();
        }

        public Set Get(long setId)
        {
            if (setId == 0)
            {
                return null;
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(set => set.SetId == setId)
                .FirstOrDefault();
        }

        public IEnumerable<Set> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(set => set.Theme.Name == themeName.Trim())
                .ToList();
        }

        public IEnumerable<Set> AllForSubtheme(string themeName, string subthemeName)
        {
            if (string.IsNullOrWhiteSpace(themeName)
                || string.IsNullOrWhiteSpace(subthemeName))
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(set => set.Theme.Name == themeName.Trim() && set.Subtheme.Name == subthemeName.Trim())
                .ToList();
        }

        public IEnumerable<Set> AllForThemeGroup(string themeGroupName)
        {
            if (string.IsNullOrWhiteSpace(themeGroupName))
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(set => set.ThemeGroup.Value == themeGroupName.Trim())
                .ToList();
        }

        public IEnumerable<Set> AllForCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(set => set.Category.Value == categoryName.Trim())
                .ToList();
        }

        public IEnumerable<Set> AllForTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where("Tags[*].Value ANY = @0", tagName.Trim())
                .ToList();
        }

        public IEnumerable<Set> AllForYear(short year)
        {
            if (year < Constants.MinimumSetYear)
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(set => set.Year == year)
                .ToList();
        }

        public IEnumerable<Set> AllForPriceRange(PriceRegionEnum priceRegion, float minimumPrice, float maximumPrice)
        {
            if (minimumPrice < 0 || maximumPrice < 0)
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where("Prices[*].Region ANY = @0", priceRegion.ToString())
                .Where("Prices[*].Value ANY >= @0", minimumPrice)
                .Where("Prices[*].Value ANY <= @0", maximumPrice)
                .ToList();
        }

        public IEnumerable<Set> SearchBy(string searchQuery)
        {
            var queryBsonExpression = BuildBsonExpressionFromSearchQuery(searchQuery);

            if (queryBsonExpression == null)
            {
                return Enumerable.Empty<Set>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(queryBsonExpression)
                .ToList();
        }

        private BsonExpression BuildBsonExpressionFromSearchQuery(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return null;
            }

            var queryList = new Dictionary<string, string>();
            var searchTerms = searchQuery
                .Split(' ', '-')
                .Where(term => (term?.Trim().Length ?? 0) >= Constants.MinimumSearchQuerySize)
                .Distinct()
                .ToList();

            for (int i = 0; i < searchTerms.Count; i++)
            {
                var queryString = $"Number LIKE @{i}" +
                    $" OR Name LIKE @{i}" +
                    $" OR Ean LIKE @{i}" +
                    $" OR Upc LIKE @{i}" +
                    $" OR Theme.Name LIKE @{i}" +
                    $" OR Subtheme.Name LIKE @{i}" +
                    $" OR ThemeGroup.Value LIKE @{i}" +
                    $" OR PackagingType.Value LIKE @{i}" +
                    $" OR Category.Value LIKE @{i}" +
                    $" OR Tags[*].Value ANY LIKE @{i}";

                queryList.Add($"%{searchTerms[i]}%", queryString);
            }

            return queryList.Keys.Count == 0
                ? null
                : BsonExpression.Create(string.Join(" OR ", queryList.Values), queryList.Keys.Select(key => new BsonValue(key)).ToArray());
        }

        private ILiteQueryable<Set> GetQueryable(ILiteRepository repository)
        {
            return repository
                .Query<Set>()
                .Include(set => set.Theme)
                .Include(set => set.ThemeGroup)
                .Include(set => set.Subtheme)
                .Include(set => set.PackagingType)
                .Include(set => set.Category)
                .Include(set => set.Tags);
        }
    }
}
