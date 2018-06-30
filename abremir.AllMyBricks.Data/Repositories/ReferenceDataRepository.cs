using abremir.AllMyBricks.Data.Interfaces;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ReferenceDataRepository : IReferenceDataRepository
    {
        private readonly IRepositoryService _repositoryService;

        public ReferenceDataRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public T GetOrAdd<T>(string referenceDataValue) where T : RealmObject, IReferenceData, new()
        {
            if (string.IsNullOrWhiteSpace(referenceDataValue))
            {
                return default(T);
            }

            var repository = _repositoryService.GetRepository();

            var existingReferenceData = repository
                .All<T>()
                .Where(referenceData => referenceData.Value.Equals(referenceDataValue, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (!EqualityComparer<T>.Default.Equals(existingReferenceData, default(T)))
            {
                return existingReferenceData;
            }

            var newReferenceData = new T
            {
                Value = referenceDataValue
            };

            repository.Write(() => repository.Add<T>(newReferenceData));

            return newReferenceData;
        }
    }
}