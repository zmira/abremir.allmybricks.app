using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        public ThemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Theme AddOrUpdate(Theme theme)
        {
            if (theme == null
                || string.IsNullOrWhiteSpace(theme.Name)
                || theme.YearFrom < Constants.MinimumSetYear)
            {
                return null;
            }

            theme.TrimAllStrings();

            using var repository = _repositoryService.GetRepository();

            repository.Upsert(theme);

            return theme;
        }

        public IEnumerable<Theme> All()
        {
            using var repository = _repositoryService.GetRepository();

            return repository.Fetch<Theme>("1 = 1");
        }

        public Theme Get(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return null;
            }

            using var repository = _repositoryService.GetRepository();

            return repository.FirstOrDefault<Theme>(theme => theme.Name == themeName.Trim());
        }

        public IEnumerable<Theme> AllForYear(short year)
        {
            if (year < Constants.MinimumSetYear)
            {
                return Enumerable.Empty<Theme>();
            }

            using var repository = _repositoryService.GetRepository();

            return repository.Fetch<Theme>(theme => theme.YearFrom <= year && theme.YearTo >= year);
        }
    }
}
