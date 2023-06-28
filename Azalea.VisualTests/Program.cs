using Azalea;
using Azalea.VisualTests;

var host = Host.CreateHost(new HostPreferences { Type = HostType.Silk });
host.Run(new VisualTests());