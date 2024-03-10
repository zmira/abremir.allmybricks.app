﻿using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using LiteDB.async;
using LiteDB.Async;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class SubthemeRepository : ISubthemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        public SubthemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public async Task<Subtheme> AddOrUpdate(Subtheme subtheme)
        {
            if (subtheme is null
                || subtheme.Theme is null
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

            await repository.UpsertAsync(subtheme).ConfigureAwait(false);

            return subtheme;
        }

        public async Task<IEnumerable<Subtheme>> All()
        {
            using var repository = _repositoryService.GetRepository();

            return await GetQueryable(repository).ToListAsync().ConfigureAwait(false);
        }

        public async Task<Subtheme> Get(string themeName, string subthemeName)
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

            return await GetQueryable(repository)
                .Where(subtheme => subtheme.Theme.Name == themeName.Trim() && subtheme.Name == subthemeName.Trim())
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Subtheme>> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return [];
            }

            using var repository = _repositoryService.GetRepository();

            return await GetQueryable(repository)
                .Where(subtheme => subtheme.Theme.Name == themeName.Trim())
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Subtheme>> AllForYear(short year)
        {
            if (year < Constants.MinimumSetYear)
            {
                return [];
            }

            using var repository = _repositoryService.GetRepository();

            return await GetQueryable(repository)
                .Where(subtheme => subtheme.YearFrom <= year && subtheme.YearTo >= year)
                .ToListAsync().ConfigureAwait(false);
        }

        private ILiteQueryable<Subtheme> GetQueryable(ILiteRepository repository)
        {
            return repository
                .Query<Subtheme>()
                .IncludeAll();
        }
    }
}
