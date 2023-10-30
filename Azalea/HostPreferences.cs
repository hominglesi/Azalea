using Azalea.Platform;

namespace Azalea;

public struct HostPreferences
{
	public Vector2Int PreferredClientSize = new(1280, 720);
	public WindowState PreferredWindowState = WindowState.Normal;
	public bool WindowResizable = false;
	public string WindowTitle = "Azalea Game";

	public HostPreferences() { }
}
