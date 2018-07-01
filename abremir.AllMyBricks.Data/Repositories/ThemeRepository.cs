using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using ExpressMapper.Extensions;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        private IEnumerable<Theme> EmptyEnumerable => new Theme[] { };

        public ThemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Theme AddOrUpdate(Theme theme)
        {
            if(theme == null
                || string.IsNullOrWhiteSpace(theme.Name)
                || theme.YearFrom < Constants.MinimumSetYear)
            {
                return null;
            }

            var existingTheme = Get(theme.Name);

            var repository = _repositoryService.GetRepository();

            var managedTheme = theme.Map<Theme, Managed.Theme>();

            repository.Write(() => repository.Add(managedTheme, existingTheme != null));

            return managedTheme.Map<Managed.Theme, Theme>();
        }

        public IEnumerable<Theme> All()
        {
            return GetQueryable().Map<IQueryable<Managed.Theme>, IEnumerable<Theme>>();
        }

        public Theme Get(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return null;
            }

            return GetQueryable()
                .FirstOrDefault(theme => theme.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase))
                ?.Map<Managed.Theme, Theme>();
        }

        public IEnumerable<Theme> AllForYear(short year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"SetCountPerYear.Year == {year}")
                .Map<IQueryable<Managed.Theme>, IEnumerable<Theme>>();
        }

        private IQueryable<Managed.Theme> GetQueryable()
        {
            return _repositoryService
                .GetRepository()
                .All<Managed.Theme>();
        }
    }
}