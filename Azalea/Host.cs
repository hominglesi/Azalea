using Azalea.Platform;
using Azalea.Platform.XNA;

namespace Azalea;

public static class Host
{
    public static IGameHost CreateHost(HostPreferences preferences)
    {
        return preferences.Type switch
        {
            HostType.XNA => new XNAGameHost(),
            _ => throw new Exception($"Host of type {preferences.Type} does not exist")
        };
    }
}


