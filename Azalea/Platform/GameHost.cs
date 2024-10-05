using Azalea.Debugging;
using Azalea.Design.Containers;
using Azalea.Extentions;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.IO.Configs;
using Azalea.Physics;
using Azalea.Sounds;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Platform;

public abstract class GameHost
{
	public static GameHost Main => _main ?? throw new Exception("GameHost hasn't been created yet.");
	private static GameHost? _main;

	public abstract IWindow Window { get; }
	public abstract IRenderer Renderer { get; }
	public abstract IAudioManager AudioManager { get; }
	public abstract IConfigProvider ConfigProvider { get; }

	public IClipboard Clipboard => _clipboard ?? throw new Exception("This GameHost does not support using the clipboard.");
	private IClipboard? _clipboard;

	public event Action? Initialized;

	public PhysicsGenerator Physics => _physics ?? throw new Exception("Game has not been started yet");
	internal PhysicsGenerator? _physics;

	public Composition Root => _root ?? throw new Exception("Cannot use root before the game has started.");
	internal Composition? _root;

	protected GameHost()
	{
		_main ??= this;
	}

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

		CallInitialized();

		_lastFrameTime = GetCurrentTime();

		RunGameLoop();
	}

	private float _accumulator = 0;
	private float _targetFrameTime = 1 / (float)60;
	private DateTime _lastFrameTime;
	private DateTime _frameTime;
	private float _deltaTime;
	private bool _firstWindowShow = false;

	protected virtual void RunGameLoop()
	{
		while (Window.ShouldClose == false)
		{
			ProcessGameLoop();
		}

		Window.Hide();

		PerformanceTrace.SaveEventsTo("C:\\Programming\\trace.txt");

		Window.Dispose();
	}

	protected virtual void ProcessGameLoop()
	{
		StartFrame();

		_frameTime = GetCurrentTime();
		_deltaTime = (float)_frameTime.Subtract(_lastFrameTime).TotalSeconds;
		_lastFrameTime = _frameTime;

		Time.Update(_deltaTime);

		_accumulator += _deltaTime;

		while (_accumulator >= _targetFrameTime)
		{
			PerformanceTrace.RunAndTrace(CallOnFixedUpdate, "FixedUpdate");
			_accumulator -= _targetFrameTime;
		}

		PerformanceTrace.RunAndTrace(CallOnUpdate, "Update");
		PerformanceTrace.RunAndTrace(CallOnRender, "Render");

		if (_firstWindowShow == false)
		{
			Window.Show(true);
			_firstWindowShow = true;
		}

		Window.ProcessEvents();

		EndFrame();
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

	public virtual DateTime GetCurrentTime() => Time.GetCurrentPreciseTime();

	internal void CheckForbidden(object? preference, string errorMessage)
	{
		if (preference is not null)
			throw new ArgumentException(errorMessage);
	}
}
