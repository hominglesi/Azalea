using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Extentions;
using Azalea.Graphics;
using Azalea.IO.Configs;
using Azalea.Lists;
using Azalea.Platform.Audio;
using Azalea.Platform.Loading;
using Azalea.Platform.Loading.OpenGL;
using Azalea.Simulations;
using Azalea.Threading;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azalea.Platform;

public abstract class GameHost
{
	#region SHIPPABLE

	private static GameHost? _instance;

	public static GameHost Instance => _instance
		?? throw new InvalidOperationException("A GameHost has not yet been initialized!");

	public PlatformLoader Loader { get; }

	public PlatformAudio Audio { get; }

	#endregion

	private const float __fixedUpdateFrametime = 1f / 60;

	public readonly ObservableList<Application> Applications = [];

	public IConfigProvider? ConfigProvider { get; protected set; }
	public SceneContainer SceneManager { get; }
	public PhysicsGenerator Physics { get; }
	public IClipboard Clipboard { get; }

	private readonly Composition _root;

	internal GameHost(HostPreferences prefs)
	{
		#region SHIPPABLE

		if (_instance is not null)
			throw new InvalidOperationException("Only one instance of GameHost may be created!");

		_instance = this;

		if (NativeLibrary.TryLoad("soft_oal", out var _) == false)
		{
			throw new Exception("Native binaries could not be loaded!\n" +
				"If you are a developer make sure to specify a RuntimeIdentifier in the project. " +
				"Valid runtimes are: 'win-x64'.\n" +
				"If you are a user and have moved the executable file " +
				"make sure to move all the other files with it.");
		}

		Loader = new GLLoader();
		Audio = PlatformAudio.Create();

		#endregion

		Clipboard = CreateClipboard();
		Physics = new PhysicsGenerator();
		SceneManager = new SceneContainer();

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

	private float _accumulator;
	protected virtual void ProcessGameLoop()
	{
		_accumulator += Time.DeltaTime;

		Scheduler.InvokeScheduled();

		while (_accumulator >= __fixedUpdateFrametime)
		{
			CallOnFixedUpdate();
			_accumulator -= __fixedUpdateFrametime;
		}

		CallOnUpdate();
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

	internal abstract IClipboard CreateClipboard();
	public abstract ITrayIcon CreateTrayIcon(string iconName, Image icon);

	public virtual DateTime GetCurrentTime() => Time.GetCurrentPreciseTime();

	internal static void CheckForbidden(object? preference, string errorMessage)
	{
		if (preference is not null)
			throw new ArgumentException(errorMessage);
	}
}
