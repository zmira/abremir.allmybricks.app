namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IFileSystemService
    {
        string GetLocalPathToFile(string filename, string subfolder = null);
    }
}