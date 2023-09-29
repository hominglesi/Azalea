using Azalea.Audios;
using Azalea.Design.Containers;
using Azalea.Extentions;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Inputs;
using Azalea.IO.Stores;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Platform;

public abstract class GameHost
{
	public abstract IWindow Window { get; }
	public abstract IRenderer Renderer { get; }
	public IClipboard Clipboard => _clipboard ?? throw new Exception("This GameHost does not support using the clipboard.");
	private IClipboard? _clipboard;

	public event Action? Initialized;

	public Composition Root => _root ?? throw new Exception("Cannot use root before the game has started.");

	private Composition? _root;

	public virtual void Run(AzaleaGame game)
	{
		var root = new Composition();
		root.Add(game);

		game.SetHost(this);

		_clipboard = CreateClipboard();

		_root = root;
	}

	public virtual void CallInitialized()
	{
		Debug.Assert(_root is not null);

		Input.Initialize(_root);
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

		Input.LateUpdate();
	}

	public virtual IResourceStore<TextureUpload> CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore)
		=> new TextureLoaderStore(underlyingStore);

	protected virtual IClipboard? CreateClipboard() => null;
}
