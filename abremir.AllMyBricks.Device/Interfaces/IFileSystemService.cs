namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IFileSystemService
    {
        string ThumbnailCacheFolder { get; }
        string GetLocalPathToFile(string filename, string subfolder = null);
        string GetThumbnailFolder(string theme, string subtheme);
        void SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail);
    }
}