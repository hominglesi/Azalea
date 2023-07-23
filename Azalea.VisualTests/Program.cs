using Azalea;
using Azalea.VisualTests;

var host = Host.CreateHost(new HostPreferences { Type = HostType.Veldrid });
host.Run(new VisualTests());