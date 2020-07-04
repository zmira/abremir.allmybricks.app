using abremir.AllMyBricks.Data.Interfaces;
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
                return default;
            }

            using var repository = _repositoryService.GetRepository();

            var existingReferenceData = repository
                .Database
                .GetCollection<T>()
                .FindOne(t => t.Value == referenceDataValue.Trim());

            if (!EqualityComparer<T>.Default.Equals(existingReferenceData, default))
            {
                return new T
                {
                    Id = existingReferenceData.Id,
                    Value = existingReferenceData.Value
                };
            }

            var newReferenceData = new T
            {
                Value = referenceDataValue.Trim()
            };

            repository.Insert(newReferenceData);

            return newReferenceData;
        }
    }
}
