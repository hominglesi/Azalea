using Azalea;
using Azalea.Graphics;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.VisualTests;

//PerformanceTrace.Enabled = true;

Assets.SetupPersistentStore("Azalea.VisualTests");

Config.Load();

var host = Host.CreateHost(new HostPreferences
{
	GraphicsAPI = GraphicsAPI.Vulkan,
	ClientSize = new Vector2Int(1280, 720),
	WindowResizable = true,
	WindowTitle = "Azalea Visual Tests",
	WindowState = WindowState.Normal,
	VSync = true,
});

host.Run(new VisualTests());

Config.Save();
