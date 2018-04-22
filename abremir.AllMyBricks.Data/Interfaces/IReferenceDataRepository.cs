namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IReferenceDataRepository
    {
        T GetOrAdd<T>(string referenceDataValue) where T : IReferenceData, new();
    }
}