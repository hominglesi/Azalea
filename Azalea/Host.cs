using Azalea.Platform;
using Azalea.Platform.Silk;
using Azalea.Platform.XNA;

namespace Azalea;

public static class Host
{
    public static IGameHost CreateHost(HostPreferences preferences = default)
    {
        return preferences.Type switch
        {
            HostType.XNA => new XNAGameHost(),
            HostType.Silk => new SilkGameHost(),
            _ => throw new Exception($"Host of type {preferences.Type} does not exist")
        };
    }
}


