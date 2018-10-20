using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using Realms;
using System.Collections.Generic;
using System.Linq;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

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

            var managedSubtheme = GetManagedSubtheme(subtheme);

            repository.Write(() => repository.Add(managedSubtheme, existingSubtheme != null));

            return managedSubtheme.ToPlainObject();
        }

        private Managed.Subtheme GetManagedSubtheme(Subtheme subtheme)
        {
            var managedSubtheme = subtheme.ToRealmObject();

            var repository = _repositoryService.GetRepository();

            managedSubtheme.Theme = subtheme.Theme == null
                ? null
                : repository.All<Managed.Theme>()
                    .Filter($"Name ==[c] \"{subtheme.Theme.Name.Trim()}\"")
                    .FirstOrDefault();

            return managedSubtheme;
        }

        public IEnumerable<Subtheme> All()
        {
            return GetQueryable()
                .AsEnumerable()
                .ToPlainObjectEnumerable();
        }

        public Subtheme Get(string themeName, string subthemeName)
        {
            if (string.IsNullOrWhiteSpace(themeName)
                || string.IsNullOrWhiteSpace(subthemeName))
            {
                return null;
            }

            return GetQueryable()
                .Filter($"Name ==[c] \"{subthemeName.Trim()}\" && Theme.Name ==[c] \"{themeName.Trim()}\"")
                .FirstOrDefault()
                ?.ToPlainObject();
        }

        public IEnumerable<Subtheme> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"Theme.Name ==[c] \"{themeName.Trim()}\"")
                .AsEnumerable()
                .ToPlainObjectEnumerable();
        }

        public IEnumerable<Subtheme> AllForYear(short year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Where(subtheme => subtheme.YearFrom <= year && subtheme.YearTo >= year)
                .AsEnumerable()
                .ToPlainObjectEnumerable();
        }

        private IQueryable<Managed.Subtheme> GetQueryable()
        {
            return _repositoryService
                .GetRepository()
                .All<Managed.Subtheme>();
        }
    }
}