using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using Realms;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class SubthemeRepository : ISubthemeRepository
    {
        private readonly IRepositoryService _repositoryService;

        private IEnumerable<Subtheme> EmptyEnumerable => new Subtheme[] { };

        public SubthemeRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Subtheme AddOrUpdate(Subtheme subtheme)
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

            var existingSubtheme = Get(subtheme.Theme.Name, subtheme.Name);

            var repository = _repositoryService.GetRepository();

            repository.Write(() => repository.Add(subtheme, existingSubtheme != null));

            return subtheme;
        }

        public IEnumerable<Subtheme> All()
        {
            return GetQueryable();
        }

        public IEnumerable<Subtheme> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Filter($"Theme.Name ==[c] '{themeName}'");
        }

        public IEnumerable<Subtheme> AllForYear(short year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return EmptyEnumerable;
            }

            return GetQueryable().Where(subtheme => subtheme.YearFrom <= year && subtheme.YearTo >= year );
        }

        public Subtheme Get(string themeName, string subthemeName)
        {
            if(string.IsNullOrWhiteSpace(themeName)
                || string.IsNullOrWhiteSpace(subthemeName))
            {
                return null;
            }

            return GetQueryable().Filter($"Name ==[c] '{subthemeName}' && Theme.Name ==[c] '{themeName}'").FirstOrDefault();
        }

        private IQueryable<Subtheme> GetQueryable()
        {
            return _repositoryService.GetRepository().All<Subtheme>();
        }
    }
}