using Azalea.Graphics.Rendering;
using Microsoft.Xna.Framework;

namespace Azalea.Platform;

internal abstract class GameHost : IGameHost
{
    public abstract IRenderer Renderer { get; }

    public event Action? Initialized;
    public event Action? OnRender;

    public virtual void Run(AzaleaGame game)
    {
        game.SetHost(this);
    }

    public virtual void CallInitialized()
    {
        Renderer.Initialize();
        Initialized?.Invoke();
    }

    public virtual void CallOnRender()
    {
        if (Renderer.AutomaticallyClear) Renderer.Clear();
        OnRender?.Invoke();
        Renderer.FlushCurrentBatch();
    }
}
