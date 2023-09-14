using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Textures;
using Azalea.Inputs;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using Azalea.Platform;
using System;

namespace Azalea;

public abstract class AzaleaGame : Container
{
	public static AzaleaGame Main => _main ?? throw new Exception("No game has been initialized");
	private static AzaleaGame? _main;

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
		if (_main is null) _main = this;

		_host = host;

		Host.Initialized += CallInitialize;
	}

	public AzaleaGame()
	{
		RelativeSizeAxes = Axes.Both;
	}

	internal void CallInitialize()
	{
		_resources = new ResourceStore<byte[]>();
		_resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(AzaleaGame).Assembly), @"Resources"));

		_textures = new TextureStore(Host.Renderer,
			Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));

		_fonts = new FontStore(Host.Renderer);
		_fonts.AddStore(_localFonts = new FontStore(Host.Renderer));

		addFont(_localFonts, Resources, @"Fonts/Roboto-Regular");

		Assets.InitializeAssets(this);

		Host.Window.SetIconFromStream(Textures.GetStream("azalea-icon.png")!);

		Host.Window.Center();

		OnInitialize();
	}

	protected internal virtual UserInputManager CreateUserInputManager() => new();

	public void AddFont(ResourceStore<byte[]> store, string? assetName = null, FontStore? target = null)
		=> addFont(target ?? Fonts, store, assetName);

	private void addFont(FontStore target, ResourceStore<byte[]> store, string? assetName = null)
		=> target.AddTextureSource(new GlyphStore(store, assetName, Host.CreateTextureLoaderStore(store)));

	protected virtual void OnInitialize() { }
}
