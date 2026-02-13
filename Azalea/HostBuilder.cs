using Azalea.Platform;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Azalea;
public class HostBuilder
{
	private HostPreferences _preferences;
	public HostBuilder()
	{
		_preferences = new HostPreferences();
	}

	public GameHost Create()
	{
		if (RuntimeInformation.ProcessArchitecture == Architecture.Wasm)
		{
			var webAssembly = Assembly.Load("Azalea.Web");
			var webHostType = webAssembly.GetType("Azalea.Web.Platform.WebHost")!;
			var arguments = new object[] { _preferences };
			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			return (GameHost)Activator.CreateInstance(webHostType, bindingFlags, null, arguments, null)!;
		}

		if (NativeLibrary.TryLoad("soft_oal", out var _) == false)
		{
			throw new Exception("Native binaries could not be loaded!\n" +
				"If you are a developer make sure to specify a RuntimeIdentifier in the project. " +
				"Valid runtimes are: 'win-x64'.\n" +
				"If you are a user and have moved the executable file " +
				"make sure to move all the other files with it.");
		}

		return new DesktopGameHost(_preferences);
	}

	public HostBuilder SetGameSize(Vector2Int size)
	{
		_preferences.GameSize = size;
		return this;
	}
	public HostBuilder SetResizable(bool resizable)
	{
		_preferences.Resizable = resizable;
		return this;
	}

	public HostBuilder SetStartingState(WindowState state)
	{
		_preferences.StartingState = state;
		return this;
	}

	public HostBuilder SetTitle(string title)
	{
		_preferences.Title = title;
		return this;
	}

	public HostBuilder SetVSync(bool vSyncEnabled)
	{
		_preferences.VSync = vSyncEnabled;
		return this;
	}

	public HostBuilder SetupPersistentDirectory(string folderName)
	{
		_preferences.PersistentDirectory = folderName;
		return this;
	}

	public HostBuilder SetupReflectedDirectory(string path)
	{
		_preferences.ReflectedDirectory = path;
		return this;
	}

	public HostBuilder SetupConfig(string configName = "config.cfg")
	{
		if (_preferences.PersistentDirectory is null)
			throw new Exception("Must setup persistent directory before setting up config");

		_preferences.ConfigName = configName;
		return this;
	}

	public HostBuilder EnableEditor()
	{
		_preferences.EditorEnabled = true;
		return this;
	}

	public HostBuilder EnableTracing()
	{
		if (_preferences.PersistentDirectory is null)
			throw new Exception("Must setup persistent directory before setting up config");

		_preferences.TracingEnabled = true;
		return this;
	}
}
internal struct HostPreferences
{
	public string? ConfigName = null;
	public bool EditorEnabled = false;
	public Vector2Int? GameSize = null;
	public string? PersistentDirectory = null;
	public string? ReflectedDirectory = null;
	public bool? Resizable = null;
	public WindowState? StartingState = null;
	public string? Title = null;
	public bool TracingEnabled = false;
	public bool? VSync = null;

	public HostPreferences() { }
}
