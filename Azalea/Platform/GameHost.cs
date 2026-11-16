using Azalea.Extentions;
using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Lists;
using Azalea.Platform.Audio;
using Azalea.Platform.Loading;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Platform.Windowing;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azalea.Platform;

public abstract class GameHost
{
	private static GameHost? _instance;

	public static GameHost Instance => _instance
		?? throw new InvalidOperationException("A GameHost has not yet been initialized!");

	public static GameHost Create()
	{
		if (NativeLibrary.TryLoad("soft_oal", out var _) == false)
		{
			throw new Exception("Native binaries could not be loaded!\n" +
				"If you are a developer make sure to specify a RuntimeIdentifier in the project. " +
				"Valid runtimes are: 'win-x64'.\n" +
				"If you are a user and have moved the executable file " +
				"make sure to move all the other files with it.");
		}

		if (RuntimeInformation.ProcessArchitecture == Architecture.Wasm)
		{
			var webAssembly = Assembly.Load("Azalea.Web");
			var webHostType = webAssembly.GetType("Azalea.Web.Platform.WebHost")!;
			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			return (GameHost)Activator.CreateInstance(webHostType, bindingFlags, null, null, null)!;
		}

		return new DesktopGameHost();
	}

	internal GameHost()
	{
		if (_instance is not null)
			throw new InvalidOperationException("Only one instance of GameHost may be created!");

		_instance = this;

		Loader = CreateLoader();
		Audio = CreateAudio();

		Clipboard = CreateClipboard();
		Assets.MainStore.AddMsdfFont("Roboto-Regular", "Fonts/Roboto-Regular.csv", "Fonts/Roboto-Regular.bmp");
	}

	public PlatformLoader Loader { get; }
	protected abstract PlatformLoader CreateLoader();

	public PlatformAudio Audio { get; }
	protected abstract PlatformAudio CreateAudio();

	public IClipboard Clipboard { get; }
	protected abstract IClipboard CreateClipboard();

	public IConfigProvider? ConfigProvider { get; protected set; }

	protected virtual IConfigProvider? CreateConfigProvider(string path) => null;
	public void SetupConfig(string path = "config.cfg")
	{
		if (ConfigProvider is not null)
			throw new Exception("A config provider has already been set up!");

		ConfigProvider = CreateConfigProvider(path);
	}

	public void SaveConfig()
	{
		if (ConfigProvider is null)
			throw new Exception("A config provider has not been set up!");

		ConfigProvider.Save();
	}

	public void SetupPersistentDirectory(string path) => Assets.SetupPersistentStore(path);
	public void SetupReflectedDirectory(string path) => Assets.SetupReflectedStore(path);

	internal abstract PlatformWindow CreateWindow(Vector2Int size, bool initiallyVisible = true, bool initializeOle = true);
	internal abstract PlatformRenderer CreateRenderer(IPlatformDeviceContext deviceContext);

	public readonly ObservableList<Application> Applications = [];
	public Application StartApplication(Application application, string? title = null, Vector2Int? clientSize = null, bool visible = true)
	{
		var window = CreateWindow(clientSize ?? new(800, 600), visible);
		window.Title = title ?? "Azalea Application";
		window.Center();
		window.SetIcon(Assets.MainStore.GetImage("Textures/azalea-icon.png"));
		window.OnClosed += () => onApplicationClosed(application);

		var deviceContext = window.BorrowDeviceContext();
		var renderer = CreateRenderer(deviceContext);
		window.Subscribe(renderer);

		var scheduler = new PlatformScheduler(window);
		window.Subscribe(scheduler);

		var input = new InputProcessor(window.GamepadInput, window.CreateProxy<bool>("ShowDroppableCursor"));

		application.Start(window, renderer, scheduler, input);

		Applications.Add(application);
		return application;
	}

	private void onApplicationClosed(Application application)
	{
		Applications.Remove(application);

		if (Applications.Count == 0)
			_closed.Set();
	}

	private readonly ManualResetEvent _closed = new(false);

	public void WaitUntilClosed()
	{
		if (Applications.Count == 0)
			return;

		_closed.WaitOne();
		_closed.Reset();
	}
}
