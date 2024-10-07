using Azalea.Debugging;
using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Extentions;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.IO.Configs;
using Azalea.Physics;
using Azalea.Sounds;
using System;
using System.Numerics;

namespace Azalea.Platform;

public abstract class GameHost
{
	public static GameHost Main => _main ?? throw new Exception("GameHost hasn't been created yet.");
	private static GameHost? _main;

	public IWindow Window { get; }
	public IRenderer Renderer { get; }
	public IAudioManager AudioManager { get; }
	public IConfigProvider? ConfigProvider { get; protected set; }
	public Composition Root { get; private set; }
	public SceneContainer SceneManager { get; }
	public PhysicsGenerator Physics { get; }
	public IClipboard Clipboard { get; }

	internal DebuggingOverlay? _editor;
	private bool _editorEnabled;

	internal GameHost(HostPreferences prefs)
	{
		_main ??= this;

		_editorEnabled = prefs.EditorEnabled;
		Root = new Composition();

		Window = CreateWindow(prefs);
		Renderer = CreateRenderer(Window);
		AudioManager = CreateAudioManager();
		Clipboard = CreateClipboard();
		Physics = new PhysicsGenerator();
		SceneManager = new SceneContainer();
	}

	public virtual void Run(AzaleaGame game)
	{
		if (_editorEnabled)
			Root = Editor._overlay = _editor = new DebuggingOverlay();

		Root.Add(game);
		game.SetHost(this);
		game.AddInternal(SceneManager);

		Input.Initialize(Root);

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

	private long _frameStart;
	protected virtual void ProcessGameLoop()
	{
		_frameStart = PerformanceTrace.StartEvent();

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

		PerformanceTrace.AddEvent(_frameStart, "Frame");
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

	internal abstract IWindow CreateWindow(HostPreferences preferences);
	internal abstract IRenderer CreateRenderer(IWindow window);
	internal abstract IAudioManager CreateAudioManager();
	internal abstract IClipboard CreateClipboard();

	public virtual DateTime GetCurrentTime() => Time.GetCurrentPreciseTime();

	internal void CheckForbidden(object? preference, string errorMessage)
	{
		if (preference is not null)
			throw new ArgumentException(errorMessage);
	}
}
