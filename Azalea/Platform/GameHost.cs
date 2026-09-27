using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Editing;
using Azalea.Extentions;
using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.IO.Configs;
using Azalea.Lists;
using Azalea.Platform.Audio;
using Azalea.Simulations;
using Azalea.Sounds;
using Azalea.Threading;
using System;
using System.Numerics;
using System.Threading;

namespace Azalea.Platform;

public abstract class GameHost
{
	#region Creation

	private static GameHost? _instance;

	public static GameHost Instance => _instance
		?? throw new InvalidOperationException("A GameHost has not yet been initialized!");

	#endregion

	private const float __fixedUpdateFrametime = 1f / 60;

	public readonly ObservableList<Application> Applications = [];

	public IRenderer Renderer { get; }
	public IAudioManager AudioManager => _audioThread.AudioManager;
	public IConfigProvider? ConfigProvider { get; protected set; }
	public SceneContainer SceneManager { get; }
	public PhysicsGenerator Physics { get; }
	public IClipboard Clipboard { get; }

	private readonly Composition _root;

	private AudioThread _audioThread;
	public PlatformAudio Audio;

	internal GameHost(HostPreferences prefs)
	{
		if (_instance is not null)
			throw new InvalidOperationException("Only one instance of GameHost may be created!");

		_instance = this;

		var window = CreateWindow(prefs);
		Renderer = CreateRenderer(window);
		_audioThread = new AudioThread(this);
		Clipboard = CreateClipboard();
		Physics = new PhysicsGenerator();
		SceneManager = new SceneContainer();

		Audio = PlatformAudio.Create();

		_root = new Composition();
		Time.Setup();
	}

	public virtual void Run(AzaleaGame game)
	{
		GameObject rootObject = game;

		_root.Add(rootObject);

		game.AddInternal(SceneManager);

		RunGameLoop();
	}

	protected abstract void RunGameLoop();

	private long _frameStart;
	private float _accumulator;
	protected virtual void ProcessGameLoop()
	{
		_frameStart = PerformanceTrace.StartEvent();

		
		_accumulator += Time.DeltaTime;

		Scheduler.InvokeScheduled();

		while (_accumulator >= __fixedUpdateFrametime)
		{
			PerformanceTrace.RunAndTrace(CallOnFixedUpdate, "FixedUpdate");
			_accumulator -= __fixedUpdateFrametime;
		}

		PerformanceTrace.RunAndTrace(CallOnUpdate, "Update");

		PerformanceTrace.AddEvent(_frameStart, "Frame");
	}

	public virtual void CallOnUpdate()
	{
		_root.Size = Vector2Extentions.ComponentMax(Vector2.One, _root.Size);

		_root.UpdateSubTree();
	}

	public virtual void CallOnFixedUpdate()
	{
		_root.FixedUpdateSubTree();

		Physics.Update();
	}

	public Application CreateApplication(AzaleaGame game, string? title = null, Vector2Int? clientSize = null)
	{
		clientSize ??= new(800, 600);
		title ??= "Azalea Application";

		var application = new Application(game, title, clientSize.Value);
		Applications.Add(application);

		application.OnClosed += () => onApplicationClosed(application);

		return application;
	}

	private void onApplicationClosed(Application application)
	{
		Applications.Remove(application);

		if (Applications.Count == 0)
			_empty.Set();
	}

	private ManualResetEvent _empty = new(false);

	public void WaitUntilEmpty()
	{
		if (Applications.Count == 0)
			return;

		_empty.WaitOne();
		_empty.Reset();
	}

	internal abstract IWindow CreateWindow(HostPreferences preferences);
	internal abstract IRenderer CreateRenderer(IWindow window);
	internal abstract IAudioManager CreateAudioManager();
	internal abstract IClipboard CreateClipboard();
	public abstract ITrayIcon CreateTrayIcon(string iconName, Image icon);

	public virtual DateTime GetCurrentTime() => Time.GetCurrentPreciseTime();

	internal static void CheckForbidden(object? preference, string errorMessage)
	{
		if (preference is not null)
			throw new ArgumentException(errorMessage);
	}
}
