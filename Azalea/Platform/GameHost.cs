using Azalea.Graphics.Rendering;

namespace Azalea.Platform;

internal abstract class GameHost : IGameHost
{
    public abstract IRenderer Renderer { get; }

    public abstract event Action Initialized;

    public virtual void Run(AzaleaGame game)
    {
        game.SetHost(this);
    }
}
