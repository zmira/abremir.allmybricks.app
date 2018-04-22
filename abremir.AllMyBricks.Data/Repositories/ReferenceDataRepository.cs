using abremir.AllMyBricks.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ReferenceDataRepository : IReferenceDataRepository
    {
        private readonly IRepositoryService _repositoryService;

        public ReferenceDataRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public T GetOrAdd<T>(string referenceDataValue) where T : IReferenceData, new()
        {
            if (string.IsNullOrWhiteSpace(referenceDataValue))
            {
                return default(T);
            }

            using (var repository = _repositoryService.GetRepository())
            {
                var existingReferenceData = repository
                    .Query<T>()
                    .Where(referenceData => referenceData.Value.Equals(referenceDataValue, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (!EqualityComparer<T>.Default.Equals(existingReferenceData, default(T)))
                {
                    return existingReferenceData;
                }

                var newReferenceData = new T
                {
                    Value = referenceDataValue
                };

                repository.Insert<T>(newReferenceData);

                return newReferenceData;
            }
        }
    }
}