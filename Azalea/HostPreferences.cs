using Azalea.Platform;

namespace Azalea;

public struct HostPreferences
{
    public HostType Type = HostType.Veldrid;

    public Vector2Int PreferredClientSize = new(1280, 720);
    public WindowState PreferredWindowState = WindowState.Normal;

    public HostPreferences() { }
}

public enum HostType
{
    Silk,
    Veldrid,
    XNA,
}