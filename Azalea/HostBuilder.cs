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

		return new DesktopGameHost(_preferences);
	}

	public HostBuilder SetEditorEnabled(bool enabled)
	{
		_preferences.EditorEnabled = enabled;
		return this;
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

	public HostBuilder SetupConfig(string configName = "config.cfg")
	{
		if (_preferences.PersistentDirectory is null)
			throw new Exception("Must setup persistent directory before setting up config");

		_preferences.ConfigName = configName;
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
}
internal struct HostPreferences
{
	public string? ConfigName = null;
	public bool EditorEnabled = true;
	public Vector2Int? GameSize = null;
	public string? PersistentDirectory = null;
	public string? ReflectedDirectory = null;
	public bool? Resizable = null;
	public WindowState? StartingState = null;
	public string? Title = null;
	public bool? VSync = null;

	public HostPreferences() { }
}
