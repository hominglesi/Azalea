using Azalea.Graphics.Rendering;
using Azalea.IO.Assets;
using Microsoft.Xna.Framework;

namespace Azalea.Platform;

internal abstract class GameHost : IGameHost
{
    public abstract IRenderer Renderer { get; }

    public event Action? Initialized;
    public event Action? OnRender;
    public event Action? OnUpdate;

    public virtual void Run(AzaleaGame game)
    {
        game.SetHost(this);
    }

    public virtual void CallInitialized()
    {
        Renderer.Initialize();
        Assets.RENDERER = Renderer;
        Initialized?.Invoke();
    }

    public virtual void CallOnRender()
    {
        if (Renderer.AutomaticallyClear) Renderer.Clear();
        OnRender?.Invoke();
        Renderer.FlushCurrentBatch();
    }

    public virtual void CallOnUpdate()
    {
        OnUpdate?.Invoke();
    }
}
