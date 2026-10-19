using Azalea.IO.Configs;
using Azalea.IO.Resources;
using Azalea.Lists;
using Azalea.Platform.Audio;
using Azalea.Platform.Loading;
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
		if (_instance is not null)
			throw new InvalidOperationException("Only one instance of GameHost may be created!");

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
			_instance = (GameHost)Activator.CreateInstance(webHostType, bindingFlags, null, null, null)!;
			return _instance;
		}

		_instance = new DesktopGameHost();
		return _instance;
	}

	internal GameHost()
	{
		Loader = CreateLoader();
		Audio = CreateAudio();

		Clipboard = CreateClipboard();
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

	public readonly ObservableList<Application> Applications = [];
	public Application CreateApplication(AzaleaGame game, string? title = null, Vector2Int? clientSize = null, bool visible = true)
	{
		clientSize ??= new(800, 600);
		title ??= "Azalea Application";

		var application = new Application(game, title, clientSize.Value, visible);
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

	private readonly ManualResetEvent _empty = new(false);

	public void WaitUntilEmpty()
	{
		if (Applications.Count == 0)
			return;

		_empty.WaitOne();
		_empty.Reset();
	}
}
