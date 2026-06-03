using Azalea.Editor.Design.Gui;
using Azalea.Graphics.Colors;
using Azalea.Native.OpenGL;
using Azalea.Numerics;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Platform.Scheduling;
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
				var renderer = PlatformRenderer.AttachRenderer(window);
				var scheduler = PlatformScheduler.AttachScheduler(window);

				renderer.BeginCommandGroup();

				renderer.Disable(GL.CULL_FACE);
				renderer.Disable(GL.DEPTH_TEST);

				var defaultQuadBatch = new DefaultQuadBatch(renderer.GetCoordinator());

				renderer.PrintErrors();

				renderer.SubmitCommandGroup();

				scheduler.InjectProtocol((win, rend) =>
				{
					var renderQueue = RenderCommandQueue.Borrow();
					renderQueue.Clear(Palette.Beige);

					defaultQuadBatch.Add(new Rectangle(new(100, 100), new(50)), Palette.Beige);
					defaultQuadBatch.Add(new Rectangle(new(400, 150), new(150)), Palette.Beige);
					defaultQuadBatch.Add(new Rectangle(new(50, 300), new(200)), Palette.Beige);

					defaultQuadBatch.Draw(renderQueue);

					renderQueue.PrintErrors();

					renderQueue.SwapBuffers();

					renderer.StageQueue(renderQueue);
				});
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
