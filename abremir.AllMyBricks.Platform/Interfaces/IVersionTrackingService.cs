namespace abremir.AllMyBricks.Platform.Interfaces
{
    public interface IVersionTrackingService
    {
        bool IsFirstLaunch { get; }
    }
}
