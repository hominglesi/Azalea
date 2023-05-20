using Azalea.Platform;

namespace Azalea;

public abstract class AzaleaGame
{
    public IGameHost Host => _host ?? throw new Exception("GameHost has not been set");
    private GameHost? _host;

    internal virtual void SetHost(GameHost host)
    {
        _host = host;

        _host.Initialized += OnInitialize;
    }

    protected abstract void OnInitialize();
}
