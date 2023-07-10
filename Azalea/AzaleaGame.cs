using Azalea.Graphics.Containers;
using Azalea.Graphics.Textures;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using Azalea.Platform;
using System;

namespace Azalea;

public abstract class AzaleaGame : Container
{
    public GameHost Host => _host ?? throw new Exception("GameHost has not been set");
    private GameHost? _host;

    public ResourceStore<byte[]> Resources => _resources ?? throw new Exception("Game has not been initialized");
    private ResourceStore<byte[]>? _resources;
    public TextureStore Textures => _textures ?? throw new Exception("Game has not been initialized");
    private TextureStore? _textures;

    internal virtual void SetHost(GameHost host)
    {
        _host = host;

        Host.Initialized += CallInitialize;
        Host.OnUpdate += OnUpdate;
    }

    internal void CallInitialize()
    {
        _resources = new ResourceStore<byte[]>();
        _resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(AzaleaGame).Assembly), "Resources"));

        _textures = new TextureStore(Host.Renderer,
            Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, "Textures")));

        Host.Window.SetIconFromStream(Textures.GetStream("Resources/azalea-icon.png")!);

        Assets.InitializeAssets(this);

        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    protected virtual void OnUpdate() { }
}
