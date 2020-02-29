namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IReferenceDataRepository
    {
        T GetOrAdd<T>(string value) where T : IReferenceData, new();
    }
}
