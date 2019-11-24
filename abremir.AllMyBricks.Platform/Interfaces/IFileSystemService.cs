using System.Threading.Tasks;

namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface IFileSystemService
    {
        void EnsureLocalDataFolder(string folder = null);
        string ThumbnailCacheFolder { get; }
        string GetLocalPathToFile(string filename, string subfolder = null);
        string GetLocalPathToDataFolder();
        string GetThumbnailFolder(string theme, string subtheme);
        Task SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail);
        bool ClearThumbnailCache();
    }
}
