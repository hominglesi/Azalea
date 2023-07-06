using Azalea.Graphics.Rendering;
using System;

namespace Azalea.Platform;

public interface IGameHost
{
    public IWindow Window { get; }
    public IRenderer Renderer { get; }

    internal event Action Initialized;

    public void Run(AzaleaGame game);
}
