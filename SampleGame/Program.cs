﻿using Azalea;
using SampleGame;

var host = Host.CreateHost(new HostPreferences { Type = HostType.Veldrid, PreferredClientSize = new Vector2Int(876, 660) });
host.Run(new MemoryGame());