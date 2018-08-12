using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystem _fileSystem;

        public FileSystemService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string GetLocalPathToFile(string filename, string subfolder = null)
        {
            return $"{_fileSystem.AppDataDirectory}{Constants.AllMyBricksDataFolder}/{(string.IsNullOrWhiteSpace(subfolder?.Trim()) ? string.Empty : $"{subfolder.Trim()}/")}{(filename ?? string.Empty).Trim()}";
        }
    }
}