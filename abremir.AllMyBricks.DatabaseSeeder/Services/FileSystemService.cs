using abremir.AllMyBricks.Device.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class FileSystemService : IFileSystemService
    {
        public string ThumbnailCacheFolder => string.Empty;

        private const string DataFolder = "data";

        public bool ClearThumbnailCache()
        {
            return true;
        }

        public void EnsureLocalDataFolder()
        {
            var localDataFolder = GetLocalPathToFile(null);

            if (!Directory.Exists(localDataFolder))
            {
                Directory.CreateDirectory(localDataFolder);
            }
        }

        public string GetLocalPathToFile(string filename, string subfolder = null)
        {
            return Path.Combine(Directory.GetCurrentDirectory(),
                DataFolder,
                string.IsNullOrWhiteSpace(subfolder?.Trim()) ? string.Empty : subfolder.Trim(),
                (filename ?? string.Empty).Trim());
        }

        public string GetThumbnailFolder(string theme, string subtheme)
        {
            return string.Empty;
        }

        public Task SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail)
        {
            return null;
        }
    }
}