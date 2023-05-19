using Azalea.Platform;
using Azalea.Platform.XNA;

namespace Azalea;

public static class Host
{
    public static IGameHost CreateHost(HostType type)
    {
        return type switch
        {
            HostType.XNA => new XNAGameHost(),
            _ => throw new Exception($"Host of type {type} does not exist")
        };
    }
}

public enum HostType
{
    XNA
}
