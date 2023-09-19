using Azalea.Audios;
using Azalea.Extentions;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.IO.Stores;
using System;
using System.Numerics;

namespace Azalea.Platform;

public abstract class GameHost
{
	public abstract IWindow Window { get; }
	public abstract IRenderer Renderer { get; }
	public IClipboard Clipboard => _clipboard ?? throw new Exception("This GameHost does not support using the clipboard.");
	private IClipboard? _clipboard;

	public event Action? Initialized;

	public Container Root => _root ?? throw new Exception("Cannot use root before the game has started.");

	private Container? _root;

	public virtual void Run(AzaleaGame game)
	{
		var root = game.CreateUserInputManager();
		root.Add(game);

		game.SetHost(this);

		_clipboard = CreateClipboard();

		_root = root;
	}

	public virtual void CallInitialized()
	{
		Renderer.Initialize();
		Audio.Initialize();
		Initialized?.Invoke();
	}

	public virtual void CallOnRender()
	{
		Renderer.BeginFrame();
		if (Renderer.AutomaticallyClear) Renderer.Clear();

		var node = Root.GenerateDrawNodeSubtree();
		node?.Draw(Renderer);

		Renderer.FinishFrame();
	}

	public virtual void CallOnUpdate()
	{
		Root.Size = new Vector2(Window.ClientSize.X, Window.ClientSize.Y);
		Root.Size = Vector2Extentions.ComponentMax(Vector2.One, Root.Size);

		Root.UpdateSubTree();
	}

	public virtual IResourceStore<TextureUpload> CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
		=> new TextureLoaderStore(underlyingStore);

	protected virtual IClipboard? CreateClipboard() => null;
}
