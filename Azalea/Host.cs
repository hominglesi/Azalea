using Azalea.Platform;
using Azalea.Platform.Silk;
using Azalea.Platform.Veldrid;
using Azalea.Platform.XNA;
using System;

namespace Azalea;

public static class Host
{
    public static GameHost CreateHost(HostPreferences preferences = default)
    {
        return preferences.Type switch
        {
            HostType.XNA => new XNAGameHost(preferences),
            HostType.Silk => new SilkGameHost(preferences),
            HostType.Veldrid => new VeldridGameHost(preferences),
            _ => throw new Exception($"Host of type {preferences.Type} does not exist")
        };
    }
}


