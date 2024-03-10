using System.Threading.Tasks;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IReferenceDataRepository
    {
        Task<T> GetOrAdd<T>(string value) where T : IReferenceData, new();
    }
}
