using System.Threading.Tasks;

namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IFileSystemService
    {
        void EnsureLocalDataFolder();
        string ThumbnailCacheFolder { get; }
        string GetLocalPathToFile(string filename, string subfolder = null);
        string GetLocalPathToDataFolder();
        string GetThumbnailFolder(string theme, string subtheme);
        Task SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail);
        bool ClearThumbnailCache();
    }
}