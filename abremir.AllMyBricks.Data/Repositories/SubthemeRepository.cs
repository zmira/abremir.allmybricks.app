using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class SubthemeRepository : ISubthemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        public SubthemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Subtheme AddOrUpdate(Subtheme subtheme)
        {
            if (subtheme == null
                || subtheme.Theme == null
                || string.IsNullOrWhiteSpace(subtheme.Theme.Name)
                || subtheme.YearFrom < Constants.MinimumSetYear
                || subtheme.Theme.YearFrom < Constants.MinimumSetYear)

            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(subtheme.Name))
            {
                subtheme.Name = "{None}";
            }

            subtheme.TrimAllStrings();

            using var repository = _repositoryService.GetRepository();

            repository.Upsert(subtheme);

            return subtheme;
        }

        public IEnumerable<Subtheme> All()
        {
            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository).ToList();
        }

        public Subtheme Get(string themeName, string subthemeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(subthemeName))
            {
                subthemeName = "{None}";
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(subtheme => subtheme.Theme.Name == themeName.Trim() && subtheme.Name == subthemeName.Trim())
                .FirstOrDefault();
        }

        public IEnumerable<Subtheme> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return Enumerable.Empty<Subtheme>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(subtheme => subtheme.Theme.Name == themeName.Trim())
                .ToList();
        }

        public IEnumerable<Subtheme> AllForYear(short year)
        {
            if (year < Constants.MinimumSetYear)
            {
                return Enumerable.Empty<Subtheme>();
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(subtheme => subtheme.YearFrom <= year && subtheme.YearTo >= year)
                .ToList();
        }

        private ILiteQueryable<Subtheme> GetQueryable(ILiteRepository repository)
        {
            return repository
                .Query<Subtheme>()
                .Include(subtheme => subtheme.Theme);
        }
    }
}
