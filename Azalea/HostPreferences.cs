using Azalea.Platform;

namespace Azalea;

public struct HostPreferences
{
	public Vector2Int ClientSize = new(1280, 720);
	public WindowState WindowState = WindowState.Normal;
	public bool WindowResizable = false;
	public string WindowTitle = "Azalea Game";
	public bool VSync = true;
	public bool WindowVisible = false;

	public HostPreferences() { }
}
