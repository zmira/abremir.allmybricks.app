using abremir.AllMyBricks.Device.Interfaces;

namespace abremir.AllMyBricks.DatabaseFeeder.Implementations
{
    public class FileSystemService : IFileSystemService
    {
        public string ThumbnailCacheFolder => string.Empty;

        public bool ClearThumbnailCache()
        {
            return true;
        }

        public string GetLocalPathToFile(string filename, string subfolder = null)
        {
            return string.Empty;
        }

        public string GetThumbnailFolder(string theme, string subtheme)
        {
            return string.Empty;
        }

        public void SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail)
        {
        }
    }
}