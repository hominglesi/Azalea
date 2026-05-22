using Azalea.Platform.Windowing.Windows;
using Azalea.Threading;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windowing;
internal abstract class PlatformWindow
{
	#region Creation

	private static List<PlatformWindow> _windows = [];
	public static event Action<PlatformWindow>? OnWindowCreated;

	protected PlatformWindow()
	{
		_thread = new WindowThread(this);
		_thread.Start();
	}

	public static PlatformWindow Create()
	{
		var newWindow = RuntimeInformation.ProcessArchitecture switch
		{
			Architecture.X64 or Architecture.X86 => new WindowsWindow(),
			_ => throw new NotSupportedException(
				$"Platform '{RuntimeInformation.ProcessArchitecture}' is not supported")
		};

		_windows.Add(newWindow);
		OnWindowCreated?.Invoke(newWindow);
		return newWindow;
	}

	public bool Initialized { get; private set; } = false;
	protected abstract void Initialize();
	protected abstract void Update();

	#endregion

	#region Thread

	private readonly WindowThread _thread;

	class WindowThread(PlatformWindow window) : GameThread(1)
	{
		public override string DisplayName => "Window Thread";

		private readonly PlatformWindow _window = window;

		protected override void Work()
		{
			if (_window.Initialized == false)
			{
				_window.Initialize();
				_window.Initialized = true;
			}

			_window.Update();
		}
	}

	#endregion

	#region Closing

	public bool Closed = false;

	public static event Action<PlatformWindow>? OnWindowClosed;
	public void Close()
	{
		if (Closed) return;

		_thread.Stop();

		Closed = true;
		_windows.Remove(this);
		OnWindowClosed?.Invoke(this);
	}

	#endregion

	public string Title => "Azalea Window";
	public virtual string PlatformType => "Abstract Window";
}
