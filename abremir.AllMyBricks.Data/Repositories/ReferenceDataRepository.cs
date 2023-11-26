using System.Collections.Generic;
using System.Linq;
using abremir.AllMyBricks.Data.Interfaces;

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

            // HACK
            // for some reason some values are not found via a normal query!
            // in this case the tag "USA", which means an insertion of a second
            // item will be attempted. as fallback use in-memory search
            if (existingReferenceData is null)
            {
                var allDocuments = repository
                    .Database
                    .GetCollection<T>()
                    .FindAll();

                existingReferenceData = allDocuments
                    .FirstOrDefault(t => t.Value == referenceDataValue.Trim());
            }

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
