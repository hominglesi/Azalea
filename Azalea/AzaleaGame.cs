using Azalea.Platform;

namespace Azalea;

public abstract class AzaleaGame
{
    internal GameHost Host => _host ?? throw new Exception("GameHost has not been set");
    private GameHost? _host;

    internal virtual void SetHost(GameHost host)
    {
        _host = host;
    }
}
