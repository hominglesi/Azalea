using Azalea;
using Azalea.Platform;
using Azalea.VisualTests;

//PerformanceTrace.Enabled = true;

var host = Host.CreateHost(new HostPreferences
{
	ClientSize = new Vector2Int(1280, 720),
	WindowResizable = true,
	WindowTitle = "Azalea Visual Tests",
	WindowState = WindowState.Normal,
	VSync = true,
});

host.Run(new VisualTests());
