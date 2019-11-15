namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IVersionTrackingService
    {
        bool IsFirstLaunch { get; }
    }
}
