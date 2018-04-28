using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ThemeYearCountRepository : IThemeYearCountRepository
    {
        private readonly IRepositoryService _repositoryService;

        public ThemeYearCountRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public ThemeYearCount AddOrUpdateThemeYearCount(ThemeYearCount themeYearCount)
        {
            if(themeYearCount == null
                || themeYearCount.Theme == null
                || string.IsNullOrWhiteSpace(themeYearCount.Theme.Name)
                || themeYearCount.Year < Constants.MinimumSetYear
                || themeYearCount.Theme.YearFrom < Constants.MinimumSetYear
                || themeYearCount.Year < themeYearCount.Theme.YearFrom
                || themeYearCount.Year > themeYearCount.Theme.YearTo)
            {
                return null;
            }

            var existingThemeYearCount = GetThemeYearCount(themeYearCount.Theme.Name, themeYearCount.Year);

            using(var repository = _repositoryService.GetRepository())
            {
                if(existingThemeYearCount == null)
                {
                    repository.Insert(themeYearCount);
                }
                else
                {
                    repository.Update(themeYearCount);
                }
            }

            return themeYearCount;
        }

        public IEnumerable<ThemeYearCount> GetAllThemeYearCount()
        {
            using(var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<ThemeYearCount>()
                    .Include(themeYearCount => themeYearCount.Key.Theme)
                    .ToEnumerable();
            }
        }

        public IEnumerable<ThemeYearCount> GetAllThemeYearCountForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return new ThemeYearCount[] { };
            }

            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<ThemeYearCount>()
                    .Where(themeYearCount => themeYearCount.Theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase))
                    .Include(themeYearCount => themeYearCount.Key.Theme)
                    .ToEnumerable();
            }
        }

        public IEnumerable<ThemeYearCount> GetAllThemeYearCountForYear(ushort year)
        {
            if (year < Constants.MinimumSetYear)
            {
                return new ThemeYearCount[] { };
            }

            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<ThemeYearCount>()
                    .Where(themeYearCount => themeYearCount.Year == year)
                    .Include(themeYearCount => themeYearCount.Key.Theme)
                    .ToEnumerable();
            }
        }

        public ThemeYearCount GetThemeYearCount(string themeName, ushort year)
        {
            if (string.IsNullOrWhiteSpace(themeName)
                || year < Constants.MinimumSetYear)
            {
                return null;
            }

            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<ThemeYearCount>()
                    .Where(themeYearCount => themeYearCount.Theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase)
                        && themeYearCount.Year == year)
                    .Include(themeYearCount => themeYearCount.Key.Theme)
                    .FirstOrDefault();
            }
        }
    }
}