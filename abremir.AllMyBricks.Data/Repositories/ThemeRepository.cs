using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;

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

            repository.Write(() => repository.Add(theme, existingTheme != null));

            return theme;
        }

        public IEnumerable<Theme> All()
        {
            return GetQueryable();
        }

        public IEnumerable<Theme> AllForYear(short year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"SetCountPerYear.Year == {year}");
        }

        public Theme Get(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return null;
            }

            return GetQueryable().Where(theme => theme.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        private IQueryable<Theme> GetQueryable()
        {
            return _repositoryService.GetRepository().All<Theme>();
        }
    }
}