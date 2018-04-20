using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using System;
using System.Collections.Generic;

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
                || string.IsNullOrWhiteSpace(subtheme.Theme.Name))
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
            using(var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Subtheme>()
                    .Include(subtheme => subtheme.Theme)
                    .ToEnumerable();
            }
        }

        public IEnumerable<Subtheme> GetAllSubthemesForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return new List<Subtheme>();
            }

            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Subtheme>()
                    .Where(subtheme => subtheme.Theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase))
                    .Include(subtheme => subtheme.Theme)
                    .ToEnumerable();
            }
        }

        public IEnumerable<Subtheme> GetAllSubthemesForYear(ushort year)
        {
            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Subtheme>()
                    .Where(subtheme => subtheme.YearFrom <= year
                        && year <= subtheme.YearTo)
                    .Include(subtheme => subtheme.Theme)
                    .ToEnumerable();
            }
        }

        public Subtheme GetSubtheme(string themeName, string subthemeName)
        {
            if(string.IsNullOrWhiteSpace(themeName)
                || string.IsNullOrWhiteSpace(subthemeName))
            {
                return null;
            }

            using (var repository = _repositoryService.GetRepository())
            {
                return repository
                    .Query<Subtheme>()
                    .Where(subtheme => subtheme.Name.Equals(subthemeName, StringComparison.InvariantCultureIgnoreCase)
                        && subtheme.Theme.Name.Equals(themeName, StringComparison.InvariantCultureIgnoreCase))
                    .Include(subtheme => subtheme.Theme)
                    .FirstOrDefault();
            }
        }
    }
}