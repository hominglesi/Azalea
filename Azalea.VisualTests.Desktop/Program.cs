﻿using Azalea;
using Azalea.VisualTests;

var host = Host.CreateHost(new HostPreferences { Type = HostType.Veldrid, PreferredClientSize = new Vector2Int(1280, 720) });
host.Run(new VisualTests());