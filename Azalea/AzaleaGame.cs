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

    public FontStore Fonts => _fonts ?? throw new Exception("Game has not been initialized");
    private FontStore? _fonts;

    private FontStore? _localFonts;

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

        _fonts = new FontStore(Host.Renderer);
        _fonts.AddStore(_localFonts = new FontStore(Host.Renderer));

        addFont(_localFonts, Resources, @"Resources/Fonts/Roboto-Regular");

        Assets.InitializeAssets(this);

        Host.Window.SetIconFromStream(Textures.GetStream("Resources/azalea-icon.png")!);

        OnInitialize();
    }

    public void AddFont(ResourceStore<byte[]> store, string? assetName = null, FontStore? target = null)
        => addFont(target ?? Fonts, store, assetName);

    private void addFont(FontStore target, ResourceStore<byte[]> store, string? assetName = null)
        => target.AddTextureSource(new GlyphStore(store, assetName, Host.CreateTextureLoaderStore(store)));

    protected virtual void OnInitialize() { }

    protected virtual void OnUpdate() { }
}
