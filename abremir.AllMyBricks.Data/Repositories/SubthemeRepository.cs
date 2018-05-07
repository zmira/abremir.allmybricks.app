using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class SubthemeRepository : ISubthemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        public SubthemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Subtheme AddOrUpdateSubtheme(Subtheme subtheme)
        {
            if(subtheme == null
                || string.IsNullOrWhiteSpace(subtheme.Name)
                || subtheme.Theme == null
                || string.IsNullOrWhiteSpace(subtheme.Theme.Name)
                || subtheme.YearFrom < Constants.MinimumSetYear
                || subtheme.Theme.YearFrom < Constants.MinimumSetYear)

            {
                return null;
            }

            var existingSubtheme = GetSubtheme(subtheme.Theme.Name, subtheme.Name);

            using (var repository = _repositoryService.GetRepository())
            {
                if(existingSubtheme == null)
                {
                    repository.Insert(subtheme);
                }
                else
                {
                    repository.Update(subtheme);
                }
            }

            return subtheme;
        }

        public IEnumerable<Subtheme> GetAllSubthemes()
        {
            return GetFromRepository(subtheme => true)
                .ToEnumerable();
        }

        public IEnumerable<Subtheme> GetAllSubthemesForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return new List<Subtheme>();
            }

            return GetFromRepository(subtheme => subtheme.Theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase))
                .ToEnumerable();
        }

        public IEnumerable<Subtheme> GetAllSubthemesForYear(ushort year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return new Subtheme[] { };
            }

            return GetFromRepository(
                    subtheme => subtheme.YearFrom <= year
                    && year <= subtheme.YearTo)
                .ToEnumerable();
        }

        public Subtheme GetSubtheme(string themeName, string subthemeName)
        {
            if(string.IsNullOrWhiteSpace(themeName)
                || string.IsNullOrWhiteSpace(subthemeName))
            {
                return null;
            }

            return GetFromRepository(
                    subtheme => subtheme.Name.Equals(subthemeName, StringComparison.InvariantCultureIgnoreCase)
                    && subtheme.Theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        private LiteQueryable<Subtheme> GetFromRepository(Expression<Func<Subtheme, bool>> whereExpression)
        {
            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Subtheme>()
                    .Include(subtheme => subtheme.Theme)
                    .Where(whereExpression);
            }
        }
    }
}