using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        public ThemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Theme AddOrUpdateTheme(Theme theme)
        {
            if(theme == null
                || string.IsNullOrWhiteSpace(theme.Name)
                || theme.YearFrom < Constants.MinimumSetYear)
            {
                return null;
            }

            var existingTheme = GetTheme(theme.Name);

            using (var repository = _repositoryService.GetRepository())
            {
                if(existingTheme == null)
                {
                    repository.Insert(theme);
                }
                else
                {
                    repository.Update(theme);
                }
            }

            return theme;
        }

        public IEnumerable<Theme> GetAllThemes()
        {
            return GetFromRepository()
                .ToEnumerable();
        }

        public IEnumerable<Theme> GetAllThemesForYear(ushort year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return new Theme[] { };
            }

            using (var repository = _repositoryService.GetRepository())
            {
                return BaseQuery(repository)
                    .Where(Query.EQ("SetCountPerYear[*].Year", (int)year))
                    .ToEnumerable();
            }
        }

        public Theme GetTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return null;
            }

            return GetFromRepository(theme => theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        private LiteQueryable<Theme> GetFromRepository(Expression<Func<Theme, bool>> whereExpression = null)
        {
            using (var repository = _repositoryService.GetRepository())
            {
                var query = BaseQuery(repository);

                if (whereExpression != null)
                {
                    query.Where(whereExpression);
                }

                return query;
            }
        }

        private LiteQueryable<Theme> BaseQuery(LiteRepository repository) => repository
                                                                                .Query<Theme>();
    }
}