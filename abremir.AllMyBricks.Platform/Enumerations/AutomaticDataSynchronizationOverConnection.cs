using System.ComponentModel;

namespace abremir.AllMyBricks.Platform.Enumerations
{
    public enum AutomaticDataSynchronizationOverConnection
    {
        [Description("Never")]
        Never,
        [Description("Only over WiFi/Ethernet connection")]
        OnlyOverWiFiConnection,
        [Description("Over any type of connection")]
        OverAnyConnection
    }
}
