using Azalea;
using Azalea.VisualTests;
using Azalea.Web.Platform;

new WebHost(new HostPreferences { WindowTitle = "Azalea Visual Tests" }).Run(new VisualTests());
