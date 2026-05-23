using Azalea.Editor.Design.Gui;
using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azalea.Editor.DebugWindows;
internal class WindowingWindow
{
	private static GUIWindow? _window;
	private static bool _shown = false;

	public static void Toggle()
	{
		_shown = !_shown;
		if (_shown) show();
		else hide();
	}

	private static readonly Dictionary<PlatformWindow, GUIGroup> _windowGroups = [];

	private static void show()
	{
		if (_window is null)
		{
			_window = GUIWindow.Create("Windows", new(400, 400));

			PlatformWindow.OnWindowCreated += window =>
			{
				var group = _window.AddGroup(window.Title);
				_window.AddLabel("Window Type: " + window.PlatformType);

				if (window is WindowsWindow win)
				{
					// A bit hacky but it's for debugging purposes so it should be fine
					while (window.Initialized == false)
						Thread.Sleep(1);

					_window.AddLabel("Class Atom: " + win.ClassAtom);
					_window.AddLabel("Handle: " + win.Handle);
				}

				_window.AddButton("Close", () => window.Close());
				_window.FinishGroup();
				_windowGroups.Add(window, group);
			};
			PlatformWindow.OnWindowClosed += window =>
			{
				_window.RemoveElement(_windowGroups[window]);
				_windowGroups.Remove(window);
			};

			_window.AddLabel("Process Architecture: " + RuntimeInformation.ProcessArchitecture);
			_window.AddButton("Create new Window", () => PlatformWindow.Create());
			_window.AddButton("Create renderable new Window", () =>
			{
				var window = PlatformWindow.Create();
				PlatformRenderer.AttachRenderer(window);
			});
		}

		_window.Show();
	}

	private static void hide()
	{
		if (_window is null)
			return;

		_window.Hide();
	}
}
