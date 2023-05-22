using Azalea.Graphics.Rendering;

namespace Azalea.Platform;

public interface IGameHost
{
    public IRenderer Renderer { get; }

    internal event Action Initialized;
    internal event Action OnRender;

    public void Run(AzaleaGame game);
}
