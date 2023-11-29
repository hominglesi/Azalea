//using Azalea.Audios;
using Azalea.Audio;
using Azalea.Debugging;
using Azalea.Design.Containers;
using Azalea.Extentions;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.IO.Resources;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Platform;

public abstract class GameHost
{
	public abstract IWindow Window { get; }
	public abstract IRenderer Renderer { get; }
	internal abstract IInputManager InputManager { get; }

	public IClipboard Clipboard => _clipboard ?? throw new Exception("This GameHost does not support using the clipboard.");
	private IClipboard? _clipboard;

	public event Action? Initialized;

	public Composition Root => _root ?? throw new Exception("Cannot use root before the game has started.");

	private Composition? _root;

	private Stopwatch _stopwatch = new();

	public virtual void Run(AzaleaGame game)
	{
		var root = new DebuggingOverlay();
		root.Add(game);

		Editor._overlay = root;

		game.SetHost(this);

		_clipboard = CreateClipboard();

		_root = root;

		//Game Loop
		CallInitialized();
		while (Window.ShouldClose == false)
		{
			if (_stopwatch.IsRunning == false) _stopwatch.Start();

			CallOnUpdate();

			CallOnRender();

			InputManager.ProcessInputs();

			Time._deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
			_stopwatch.Restart();
		}

		Window.Dispose();
		AudioManager.Dispose();
		Assets.DisposeAssets();
	}

	public virtual void CallInitialized()
	{
		Debug.Assert(_root is not null);

		Input.Initialize(_root);
		Renderer.Initialize();
		AudioManager.Initialize();
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

	protected virtual IClipboard? CreateClipboard() => null;
}
