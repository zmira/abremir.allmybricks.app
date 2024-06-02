using System.IO;
using System.Threading.Tasks;
using abremir.AllMyBricks.Platform.Configuration;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Maui.Storage;

namespace abremir.AllMyBricks.Platform.Services
{
    public class FileSystemService(
        IFileSystem fileSystem,
        IFile file) : IFileSystemService
    {
        private readonly IFileSystem _fileSystem = fileSystem;
        private readonly IFile _file = file;

        public string ThumbnailCacheFolder => Path.Combine(_fileSystem.AppDataDirectory, Constants.AllMyBricksDataFolder, Constants.ThumbnailCacheFolder);

        public string GetLocalPathToFile(string filename, string subFolder = null)
        {
            return Path.Combine(_fileSystem.AppDataDirectory,
                Constants.AllMyBricksDataFolder,
                string.IsNullOrWhiteSpace(subFolder?.Trim()) ? string.Empty : subFolder.Trim(),
                (filename ?? string.Empty).Trim());
        }

        public string GetThumbnailFolder(string theme, string subtheme)
        {
            return Path.Combine(ThumbnailCacheFolder,
                string.IsNullOrWhiteSpace(theme?.Trim()) ? Constants.FallbackFolderName : theme.Trim(),
                string.IsNullOrWhiteSpace(subtheme?.Trim()) ? Constants.FallbackFolderName : subtheme.Trim());
        }

        public async Task SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail)
        {
            if (string.IsNullOrWhiteSpace(filename) || thumbnail is null || thumbnail.Length is 0)
            {
                return;
            }

            var thumbnailFolder = GetThumbnailFolder(theme, subtheme);

            if (!Directory.Exists(thumbnailFolder))
            {
                Directory.CreateDirectory(thumbnailFolder);
            }

            await _file.WriteAllBytes(Path.Combine(thumbnailFolder, filename), thumbnail).ConfigureAwait(false);
        }

        public bool ClearThumbnailCache()
        {
            try
            {
                Directory.Delete(ThumbnailCacheFolder, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void EnsureLocalDataFolder(string folder = null)
        {
            var localDataFolder = GetLocalPathToDataFolder();

            if (!Directory.Exists(localDataFolder))
            {
                Directory.CreateDirectory(localDataFolder);
            }
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
