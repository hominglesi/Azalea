using Azalea.Graphics.Containers;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.IO.Stores;
using System;

namespace Azalea.Platform;

public abstract class GameHost
{
    public abstract IWindow Window { get; }
    public abstract IRenderer Renderer { get; }

    public event Action? Initialized;

    public Container Root => _root ?? throw new Exception("Cannot use root before the game has started");

    private Container? _root;

    public virtual void Run(AzaleaGame game)
    {
        var root = new Container();
        root.Add(game);

        game.SetHost(this);

        _root = root;
    }

    public virtual void CallInitialized()
    {
        Renderer.Initialize();
        Initialized?.Invoke();
    }

    public virtual void CallOnRender()
    {
        if (Renderer.AutomaticallyClear) Renderer.Clear();

        var node = Root.GenerateDrawNodeSubtree();
        node?.Draw(Renderer);

        Renderer.FlushCurrentBatch();
    }

    public virtual void CallOnUpdate()
    {
        Root.UpdateSubTree();
    }

    public virtual IResourceStore<TextureUpload> CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
        => new TextureLoaderStore(underlyingStore);
}
