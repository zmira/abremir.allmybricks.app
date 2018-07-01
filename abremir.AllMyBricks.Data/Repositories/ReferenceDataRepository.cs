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
    public class ReferenceDataRepository : IReferenceDataRepository
    {
        private readonly IRepositoryService _repositoryService;


        public ReferenceDataRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public T GetOrAdd<T>(string value) where T: IReferenceData
        {
            var tType = typeof(T);

            if(tType == typeof(Category))
            {
                return (T) Convert.ChangeType(GetOrAdd<Category, Managed.Category>(value), typeof(T));
            }

            if (tType == typeof(PackagingType))
            {
                return (T) Convert.ChangeType(GetOrAdd<PackagingType, Managed.PackagingType>(value), typeof(T));
            }

            if (tType == typeof(Tag))
            {
                return (T) Convert.ChangeType(GetOrAdd<Tag, Managed.Tag>(value), typeof(T));
            }

            if(tType == typeof(ThemeGroup))
            {
                return (T) Convert.ChangeType(GetOrAdd<ThemeGroup, Managed.ThemeGroup>(value), typeof(T));
            }

            return default(T);
        }

        private T GetOrAdd<T, U>(string referenceDataValue) where T : IReferenceData where U : RealmObject, IReferenceData, new()
        {
            if (string.IsNullOrWhiteSpace(referenceDataValue))
            {
                return default(T);
            }

            var repository = _repositoryService.GetRepository();

            var existingReferenceData = repository
                .All<U>()
                .Where(referenceData => referenceData.Value.Equals(referenceDataValue, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (!EqualityComparer<U>.Default.Equals(existingReferenceData, default(U)))
            {
                return existingReferenceData.Map<U, T>();
            }

            var newReferenceData = new U
            {
                Value = referenceDataValue
            };

            repository.Write(() => repository.Add<U>(newReferenceData));

            return newReferenceData.Map<U, T>();
        }
    }
}