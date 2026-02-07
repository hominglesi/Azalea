using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Editing;
using Azalea.Extentions;
using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.IO.Configs;
using Azalea.Simulations;
using Azalea.Sounds;
using System;
using System.Numerics;

namespace Azalea.Platform;

public abstract class GameHost
{
	private const float __fixedUpdateFrametime = 1f / 60;

	public static GameHost Main => _main ?? throw new Exception("GameHost hasn't been created yet.");
	private static GameHost? _main;

	public IWindow Window { get; }
	public IRenderer Renderer { get; }
	public IAudioManager AudioManager { get; }
	public IConfigProvider? ConfigProvider { get; protected set; }
	public SceneContainer SceneManager { get; }
	public PhysicsGenerator Physics { get; }
	public IClipboard Clipboard { get; }

	private readonly Composition _root;
	private readonly bool _editorEnabled;
	internal EditorContainer? EditorContainer { get; private set; }

	internal GameHost(HostPreferences prefs)
	{
		_editorEnabled = prefs.EditorEnabled;
		_main ??= this;

		Window = CreateWindow(prefs);
		Renderer = CreateRenderer(Window);
		AudioManager = CreateAudioManager();
		Clipboard = CreateClipboard();
		Physics = new PhysicsGenerator();
		SceneManager = new SceneContainer();

		_root = new Composition();
	}

	public virtual void Run(AzaleaGame game)
	{
		GameObject rootObject = game;
		Renderer.Initialize();

		if (_editorEnabled)
			rootObject = EditorContainer = new EditorContainer(game);

		_root.Add(rootObject);

		game.AddInternal(SceneManager);

		Input.Initialize(_root);
		Time.Setup();

		RunGameLoop();
	}

	private bool _firstWindowShown = false;

	protected abstract void RunGameLoop();

	private long _frameStart;
	private float _accumulator;
	protected virtual void ProcessGameLoop()
	{
		_frameStart = PerformanceTrace.StartEvent();

		Time.UpdateDeltaTime();
		_accumulator += Time.DeltaTime;

		Window.ProcessEvents();
		AudioManager.Update();

		while (_accumulator >= __fixedUpdateFrametime)
		{
			PerformanceTrace.RunAndTrace(CallOnFixedUpdate, "FixedUpdate");
			_accumulator -= __fixedUpdateFrametime;
		}

		PerformanceTrace.RunAndTrace(CallOnUpdate, "Update");
		PerformanceTrace.RunAndTrace(CallOnRender, "Render");

		if (_firstWindowShown == false)
		{
			Window.Show(true);
			_firstWindowShown = true;
		}

		PerformanceTrace.AddEvent(_frameStart, "Frame");
	}

	public virtual void CallOnRender()
	{
		Renderer.BeginFrame();
		if (Renderer.AutomaticallyClear) Renderer.Clear();

		_root.Draw(Renderer);

		Renderer.FinishFrame();
	}

	public virtual void CallOnUpdate()
	{
		_root.Size = new Vector2(Window.ClientSize.X, Window.ClientSize.Y);
		_root.Size = Vector2Extentions.ComponentMax(Vector2.One, _root.Size);

		_root.UpdateSubTree();

		Input.LateUpdate();
	}

	public virtual void CallOnFixedUpdate()
	{
		_root.FixedUpdateSubTree();

		Physics.Update();
	}

	internal abstract IWindow CreateWindow(HostPreferences preferences);
	internal abstract IRenderer CreateRenderer(IWindow window);
	internal abstract IAudioManager CreateAudioManager();
	internal abstract IClipboard CreateClipboard();

	public virtual DateTime GetCurrentTime() => Time.GetCurrentPreciseTime();

	internal static void CheckForbidden(object? preference, string errorMessage)
	{
		if (preference is not null)
			throw new ArgumentException(errorMessage);
	}
}
