using Azalea.Audio;
using Azalea.Debugging;
using Azalea.Design.Containers;
using Azalea.Extentions;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Physics;
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

	public PhysicsGenerator Physics => _physics ?? throw new Exception("Game has not been started yet");
	private PhysicsGenerator? _physics;

	public Composition Root => _root ?? throw new Exception("Cannot use root before the game has started.");

	private Composition? _root;

	public virtual void Run(AzaleaGame game)
	{
		if (AzaleaSettings.EnableDebugging)
		{
			var root = new DebuggingOverlay();
			_root = root;
			Editor._overlay = root;
		}
		else
			_root = new Composition();

		_root.Add(game);

		game.SetHost(this);

		_clipboard = CreateClipboard();

		_physics = new PhysicsGenerator();
		_physics.UsesGravity = false;

		float accumulator = 0;
		float targetFrameTime = 1 / (float)60;

		CallInitialized();

		DateTime lastFrameTime = Time.GetCurrentPreciseTime();
		DateTime frameTime;
		float deltaTime;
		var firstWindowShow = false;

		//Game Loop
		while (Window.ShouldClose == false)
		{
			StartFrame();

			frameTime = Time.GetCurrentPreciseTime();
			deltaTime = (float)frameTime.Subtract(lastFrameTime).TotalSeconds;
			lastFrameTime = frameTime;

			Time.Update(deltaTime);

			accumulator += deltaTime;

			while (accumulator >= targetFrameTime)
			{
				PerformanceTrace.RunAndTrace(CallOnFixedUpdate, "FixedUpdate");
				accumulator -= targetFrameTime;
			}

			PerformanceTrace.RunAndTrace(CallOnUpdate, "Update");
			PerformanceTrace.RunAndTrace(CallOnRender, "Render");

			if (firstWindowShow == false)
			{
				Window.Show(true);
				firstWindowShow = true;
			}

			Window.ProcessEvents();
			//PerformanceTrace.RunAndTrace(InputManager.ProcessInputs, "Input");

			EndFrame();
		}
		Window.Hide();

		PerformanceTrace.SaveEventsTo("C:\\Programming\\trace.txt");

		Window.Dispose();
		AudioManager.Dispose();
		Assets.DisposeAssets();
	}

	private long _frameStart;
	internal void StartFrame()
	{
		_frameStart = PerformanceTrace.StartEvent();
	}

	protected void EndFrame()
	{
		PerformanceTrace.AddEvent(_frameStart, "Frame");
	}

	public virtual void CallInitialized()
	{
		Debug.Assert(_root is not null);

		Input.Initialize(_root);
		Renderer.Initialize();
		AudioManager.Initialize();

		if (_root is DebuggingOverlay debug)
			debug.Initialize();

		Initialized?.Invoke();
	}

	public virtual void CallOnRender()
	{
		Renderer.BeginFrame();
		if (Renderer.AutomaticallyClear) Renderer.Clear();

		Root.Draw(Renderer);

		Renderer.FinishFrame();
	}

	public virtual void CallOnUpdate()
	{
		Root.Size = new Vector2(Window.ClientSize.X, Window.ClientSize.Y);
		Root.Size = Vector2Extentions.ComponentMax(Vector2.One, Root.Size);

		Root.UpdateSubTree();

		Input.LateUpdate();
	}

	public virtual void CallOnFixedUpdate()
	{
		Root.FixedUpdateSubTree();

		Physics.Update();
	}

	protected virtual IClipboard? CreateClipboard() => null;
}
