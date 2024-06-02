using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        public ThemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public async Task<Theme> AddOrUpdate(Theme theme)
        {
            if (theme is null
                || string.IsNullOrWhiteSpace(theme.Name)
                || theme.YearFrom < Constants.MinimumSetYear)
            {
                return null;
            }

            theme.TrimAllStrings();

            using var repository = _repositoryService.GetRepository();

            await repository.UpsertAsync(theme).ConfigureAwait(false);

            return theme;
        }

        public async Task<IEnumerable<Theme>> All()
        {
            using var repository = _repositoryService.GetRepository();

            return await repository.FetchAsync<Theme>("1 = 1").ConfigureAwait(false);
        }

        public async Task<Theme> Get(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return null;
            }

            using var repository = _repositoryService.GetRepository();

            return await repository.FirstOrDefaultAsync<Theme>(theme => theme.Name == themeName.Trim()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Theme>> AllForYear(short year)
        {
            if (year < Constants.MinimumSetYear)
            {
                return [];
            }

            using var repository = _repositoryService.GetRepository();

            return await repository.FetchAsync<Theme>(theme => theme.YearFrom <= year && theme.YearTo >= year).ConfigureAwait(false);
        }

        public async Task<int> DeleteMany(List<string> themeNames)
        {
            if ((themeNames?.Count ?? 0) is 0)
            {
                return 0;
            }

            themeNames = themeNames.ConvertAll(themeName => themeName.Trim());

            using var repository = _repositoryService.GetRepository();

            return await repository.DeleteManyAsync<Theme>(theme => themeNames.Contains(theme.Name)).ConfigureAwait(false);
        }

        public async Task<int> Count()
        {
            using var repository = _repositoryService.GetRepository();

            return await repository.Query<Theme>().CountAsync().ConfigureAwait(false);
        }
    }
}
