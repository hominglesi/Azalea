using System;
using System.Collections.Generic;

namespace Azalea.Platform.Windowing;
internal class PlatformWindow
{
	#region Creation

	private static List<PlatformWindow> _windows = [];
	public static event Action<PlatformWindow>? OnWindowCreated;

	private PlatformWindow() { }

	public static PlatformWindow Create()
	{
		var newWindow = new PlatformWindow();
		_windows.Add(newWindow);
		OnWindowCreated?.Invoke(newWindow);
		return newWindow;
	}

	#endregion

	#region Closing

	public bool Closed = false;

	public static event Action<PlatformWindow>? OnWindowClosed;
	public void Close()
	{
		if (Closed) return;

		Closed = true;
		_windows.Remove(this);
		OnWindowClosed?.Invoke(this);
	}

	#endregion

	public string Title => "Azalea Window";
}
