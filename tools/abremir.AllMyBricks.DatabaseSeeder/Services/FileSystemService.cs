using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using abremir.AllMyBricks.Platform.Interfaces;

namespace abremir.AllMyBricks.DatabaseSeeder.Services
{
    public class FileSystemService : IFileSystemService
    {
        public string ThumbnailCacheFolder => string.Empty;

        private const string DataFolder = "data";

        private string DataFolderOverride { get; set; }

        public bool ClearThumbnailCache()
        {
            return true;
        }

        public void EnsureLocalDataFolder(string folder = null)
        {
            DataFolderOverride = folder;

            var localDataFolder = GetLocalPathToDataFolder();

            if (!Directory.Exists(localDataFolder))
            {
                Directory.CreateDirectory(localDataFolder);
            }
        }

        public string GetLocalPathToFile(string filename, string subFolder = null)
        {
            var dataFolder = !string.IsNullOrEmpty(DataFolderOverride)
                ? DataFolderOverride
                : Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), DataFolder);

            return Path.Combine(dataFolder,
                string.IsNullOrWhiteSpace(subFolder?.Trim()) ? string.Empty : subFolder.Trim(),
                (filename ?? string.Empty).Trim());
        }

        public string GetThumbnailFolder(string theme, string subtheme)
        {
            return string.Empty;
        }

        public Task SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail)
        {
            return Task.CompletedTask;
        }

        public string GetLocalPathToDataFolder()
        {
            return GetLocalPathToFile(null);
        }

        public Stream GetStreamForLocalPathToFile(string file, string subFolder = null)
        {
            return new FileStream(GetLocalPathToFile(file, subFolder), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
    }
}
