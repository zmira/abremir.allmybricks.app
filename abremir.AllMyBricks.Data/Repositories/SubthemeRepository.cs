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

            return managedSubtheme.Map<Managed.Subtheme, Subtheme>();
        }

        private Managed.Subtheme GetManagedSubtheme(Subtheme subtheme)
        {
            var managedSubtheme = subtheme.Map<Subtheme, Managed.Subtheme>();

            var repository = _repositoryService.GetRepository();

            managedSubtheme.Theme = subtheme.Theme == null
                ? null
                : repository.All<Managed.Theme>()
                    .Filter($"Name ==[c] '{subtheme.Theme.Name}'")
                    .FirstOrDefault();

            return managedSubtheme;
        }

        public IEnumerable<Subtheme> All()
        {
            return GetQueryable().Map<IQueryable<Managed.Subtheme>, IEnumerable<Subtheme>>();
        }

        public Subtheme Get(string themeName, string subthemeName)
        {
            if (string.IsNullOrWhiteSpace(themeName)
                || string.IsNullOrWhiteSpace(subthemeName))
            {
                return null;
            }

            return GetQueryable()
                .Filter($"Name ==[c] '{subthemeName}' && Theme.Name ==[c] '{themeName}'")
                .FirstOrDefault()
                ?.Map<Managed.Subtheme, Subtheme>();
        }

        public IEnumerable<Subtheme> AllForTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Filter($"Theme.Name ==[c] '{themeName}'")
                .Map<IQueryable<Managed.Subtheme>, IEnumerable<Subtheme>>();
        }

        public IEnumerable<Subtheme> AllForYear(short year)
        {
            if(year < Constants.MinimumSetYear)
            {
                return EmptyEnumerable;
            }

            return GetQueryable()
                .Where(subtheme => subtheme.YearFrom <= year && subtheme.YearTo >= year )
                .Map<IQueryable<Managed.Subtheme>, IEnumerable<Subtheme>>();
        }

        private IQueryable<Managed.Subtheme> GetQueryable()
        {
            return _repositoryService
                .GetRepository()
                .All<Managed.Subtheme>();
        }
    }
}