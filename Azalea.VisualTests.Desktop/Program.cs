using Azalea;
using Azalea.Platform.Desktop;
using Azalea.VisualTests;
//var host = Host.CreateHost(new HostPreferences { Type = HostType.Silk, PreferredClientSize = new Vector2Int(1280, 720) });
//host.Run(new VisualTests());



var host = new DesktopGameHost(new HostPreferences { PreferredClientSize = new Vector2Int(1280, 720) });
host.Run(new VisualTests());
