using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class ReferenceDataRepository(IRepositoryService repositoryService) : IReferenceDataRepository
    {
        private readonly IRepositoryService _repositoryService = repositoryService;

        public async Task<T> GetOrAdd<T>(string referenceDataValue) where T : IReferenceData, new()
        {
            if (string.IsNullOrWhiteSpace(referenceDataValue))
            {
                return default;
            }

            using var repository = _repositoryService.GetRepository();

            var existingReferenceData = await repository
                .Database
                .GetCollection<T>()
                .FindOneAsync(t => t.Value == referenceDataValue.Trim()).ConfigureAwait(false);

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

            await repository.InsertAsync(newReferenceData).ConfigureAwait(false);

            return newReferenceData;
        }
    }
}
